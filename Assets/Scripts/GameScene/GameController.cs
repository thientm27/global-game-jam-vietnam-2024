using System;
using System.Collections;
using System.Collections.Generic;
using Audio;
using DG.Tweening;
using Services;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GameScene
{
    public class GameController : MonoBehaviour
    {
        private const string soundObjectName = "Sound";

        [SerializeField] private List<Sound> sounds;
        [SerializeField] private Music music;
        [SerializeField] private GameObject musicObject;
        private GameServices gameServices = null;

        void Awake()
        {
            if (GameObject.FindGameObjectWithTag(Constants.ServicesTag) == null)
            {
                GameObject gameServiceObject = new(nameof(GameServices))
                {
                    tag = Constants.ServicesTag
                };
                gameServices = gameServiceObject.AddComponent<GameServices>();
                DontDestroyOnLoad(musicObject);
                GameObject soundObject = new(soundObjectName);
                DontDestroyOnLoad(soundObject);
                gameServices.AddService(new AudioService(music, sounds, soundObject));
                audioService = gameServices.GetService<AudioService>();
                gameServices.AddService(new AudioService(music, sounds, soundObject));
                audioService.MusicOn = true;
                audioService.SoundOn = true;
                audioService.StopMusic();
            }
        }

        [Header("Other")]
        [SerializeField] private GameView gameView;
        [SerializeField] private GameModel gameModel;
        [SerializeField] private Transform playerPosition;
        [SerializeField] private FirstPersonController firstPersonController;
        [SerializeField] private PatientMoveController patientMoveController;
        [SerializeField] private Transform playerStartPoint;
        [SerializeField] private Transform playerEndPoint;
        [SerializeField] private Transform patientMouth;
        [SerializeField] private List<MeshRenderer> patientTooth;
        [SerializeField] private List<GameObject> missingTooth;

        // Pad rang
        [SerializeField] private Camera interactionCamera;
        [SerializeField] private Transform table;
        [SerializeField] private Transform maxSpawn;
        [SerializeField] private Transform minSpawn;
        [SerializeField] private CapsuleCollider playerCol;
        [SerializeField] private GameObject rangModel;
        [SerializeField] private GameObject doctorHand;
        [SerializeField] private Animator doctorAnimator;

        [SerializeField] private float yDraggingHeight = 1f;
        private Transform controlRang;
        private bool hitAble;
        private int hitCount;

        private AudioService audioService;

        private IEnumerator ResetHoleGame()
        {
            yield return new WaitForSeconds(Random.Range(5f, 10f));
            DisableRandomTooth(2);
            SpawnRang(2);
            hitCount = 0;
            hitAble = false;
            controlRang = null;
            StartCoroutine(PlayCinematicPatientComing());
        }

        void Start()
        {
            audioService.PlayMusic();
            patientMoveController.OpenMouse(false);
            firstPersonController.LockMove();
            firstPersonController.LockJump();
            DisableRandomTooth(2);
            SpawnRang(2);
            StartCoroutine(PlayCinematicPatientComing());
        }

        void Update()
        {
            firstPersonController.Controlling();
            if (Input.GetKeyDown(KeyCode.L))
            {
                StartCoroutine(PlayCinematicPatientComing());
            }

            if (Input.GetMouseButtonDown(0))
            {
                RayCastSelectRang();
            }

            if (controlRang)
            {
                var position = GetPositionFromCamera();
                position.x = controlRang.position.x;
                controlRang.position = position;
                RayCastInjectRang();
            }

            if (Input.GetMouseButtonUp(0))
            {
                if (controlRang)
                {
                    controlRang.position = new Vector3(
                        controlRang.transform.position.x,
                        Random.Range(minSpawn.position.y, maxSpawn.position.y),
                        Random.Range(maxSpawn.position.z, minSpawn.position.z)
                    );
                    controlRang = null;
                }
            }

            if (hitAble)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    Attack();
                }
            }
        }

        private IEnumerator reseting;
        [SerializeField] private Transform startRangRoi;

        private void Attack()
        {
            audioService.PlaySound(SoundToPlay.Hit, Random.Range(1, 4).ToString());
            audioService.PlaySound(SoundToPlay.Attack, Random.Range(1, 4).ToString());
            hitCount++;
            if (hitCount == 10)
            {
                hitAble = false;
                StartCoroutine(PlayLeaveCinematic());
                // one attck
            }

            doctorAnimator.SetBool("hit", true);
            patientMoveController.SetAnimation(PatientAnimationType.GotHit);
            if (reseting != null)
            {
                StopCoroutine(reseting);
            }

            // Rot ranng;
            var newTooth = Instantiate(
                rangModel,
                startRangRoi.position,
                Quaternion.identity
            );
            newTooth.SetActive(true);
            AddRandomForce(newTooth.transform, Random.Range(1, 5f));
            // reset
            reseting = finishHit();
            StartCoroutine(reseting);
        }

        private IEnumerator finishHit()
        {
            yield return new WaitForSeconds(0.5f);
            doctorAnimator.SetBool("hit", false);
            patientMoveController.SetAnimation(PatientAnimationType.Idle);
        }

        private void OnFinish()
        {
            StartCoroutine(PlayPaymentCinematic());
        }

        private Vector3 GetPositionFromCamera()
        {
            var ray = interactionCamera.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * 10, Color.green);
            var newWorldPosition = ray.GetPoint(yDraggingHeight);
            return newWorldPosition;
        }

        private void RayCastSelectRang()
        {
            Ray ray = interactionCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var raycastHit, 100f))
            {
                if (raycastHit.transform != null)
                {
                    if (raycastHit.transform.CompareTag("HandleRang"))
                    {
                        controlRang = raycastHit.transform;
                    }
                }
            }
        }

        private void RayCastInjectRang()
        {
            var ray = interactionCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray, 100f);
            Debug.Log("casting");
            foreach (var hit in hits)
            {
                Debug.Log(hit.transform.name);
                if (hit.transform != null && hit.transform.CompareTag("PatientRang"))
                {
                    if (missingTooth.Contains(hit.transform.gameObject))
                    {
                        audioService.PlaySound(SoundToPlay.Tooth);
                        controlRang.gameObject.SetActive(false);
                        controlRang = null;
                        var tooth = patientTooth.Find(o => o.gameObject == hit.transform.gameObject);
                        tooth.enabled = true;
                        missingTooth.Remove(hit.transform.gameObject);
                        Debug.Log(missingTooth.Count);
                        if (missingTooth.Count == 0)
                        {
                            OnFinish();
                        }
                    }
                }
            }
        }

        private void ShowTable(bool isActive = true)
        {
            table.gameObject.SetActive(isActive);
        }

        private void DisableRandomTooth(int numberToDisable)
        {
            for (int i = 0; i < patientTooth.Count - 1; i++)
            {
                int randomIndex = Random.Range(i, patientTooth.Count);
                MeshRenderer temp = patientTooth[i];
                patientTooth[i] = patientTooth[randomIndex];
                patientTooth[randomIndex] = temp;
            }

            for (int i = 0; i < numberToDisable; i++)
            {
                missingTooth.Add(patientTooth[i].gameObject);
                patientTooth[i].enabled = false;
            }
        }

        private void SpawnRang(int numberOfSpawn)
        {
            for (int i = 0; i < numberOfSpawn; i++)
            {
                var obj = Instantiate(rangModel, table);
                obj.SetActive(true);
                obj.transform.position
                    = new Vector3(
                        obj.transform.position.x,
                        Random.Range(minSpawn.position.y, maxSpawn.position.y),
                        Random.Range(maxSpawn.position.z, minSpawn.position.z)
                    );
            }
        }

        private IEnumerator PlayLeaveCinematic()
        {
            audioService.PlaySound(SoundToPlay.Talk, Random.Range(1, 4).ToString());
            yield return new WaitForSeconds(3f);
            playerCol.enabled = true;
            doctorHand.SetActive(false);
            patientMoveController.SetAnimation(PatientAnimationType.Idle);
            firstPersonController.RotateCamera(true);
            yield return StartCoroutine(patientMoveController.StartMoveFromEnd());
            yield return StartCoroutine(ResetHoleGame());
        }

        private IEnumerator PlayPaymentCinematic()
        {
            ShowTable(false);
            patientMoveController.OpenMouse(false);
            firstPersonController.ChangeCamera();
            firstPersonController.RotateCamera(true);
            firstPersonController.ChangeCameraRotateAt(patientMouth.position);

            yield return StartCoroutine(patientMoveController.SitDown());
            yield return StartCoroutine(patientMoveController.StandUp());
            yield return StartCoroutine(patientMoveController.Pay());
            audioService.PlaySound(SoundToPlay.Card);
            patientMoveController.SetAnimation(PatientAnimationType.Idle);

            yield return new WaitForSeconds(1f);
            doctorHand.SetActive(true);
            hitAble = true;
        }

        private bool isFirst = true;

        public void PlayGlassSound()
        {
            if (isFirst)
            {
                audioService.PlaySound(SoundToPlay.Glass, "1");
                isFirst = false;
            }

            audioService.PlaySound(SoundToPlay.Glass, "2");
        }

        private IEnumerator PlayCinematicPatientComing()
        {
            playerCol.enabled = true;
            yield return StartCoroutine(patientMoveController.StartMoveFromFirst());
            yield return StartCoroutine(patientMoveController.RotatePatient(playerPosition.position));
            yield return StartCoroutine(patientMoveController.SitDown());
            yield return StartCoroutine(MovePlayerToward());
            playerCol.enabled = false;
            firstPersonController.RotateCamera();
            firstPersonController.ChangeCamera();
            yield return new WaitForSeconds(1f);
            patientMoveController.OpenMouse(true);
            ShowTable();
        }

        private void AddRandomForce(Transform targetTransform, float forceMagnitude)
        {
            Rigidbody rb = targetTransform.AddComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 randomDirection = Random.onUnitSphere;
                rb.AddForce(randomDirection * forceMagnitude, ForceMode.Impulse);
                StartCoroutine(DestroyAfterDelay(targetTransform.gameObject, 5f));
            }
        }

        private IEnumerator DestroyAfterDelay(GameObject targetObject, float delay)
        {
            yield return new WaitForSeconds(delay);
            Destroy(targetObject);
        }

        private IEnumerator MovePlayerToward()
        {
            yield return playerPosition.DOMove(playerEndPoint.position, 2f)
                .SetEase(Ease.Linear)
                .WaitForCompletion();
        }
    }
}