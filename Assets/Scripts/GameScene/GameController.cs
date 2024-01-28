using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace GameScene
{
    public class GameController : MonoBehaviour
    {
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
        [SerializeField] private GameObject rangModel;
        private Transform controlRang;

        void Start()
        {
            firstPersonController.LockMove();
            firstPersonController.LockJump();
            DisableRandomTooth(7);
            SpawnRang(7);
            StartCoroutine(PlayCinematicPatientComing());
        }

        private void SpawnRang(int numberOfSpawn = 1)
        {
            for (int i = 0; i < numberOfSpawn; i++)
            {
                var obj = Instantiate(rangModel, table);
                obj.transform.position
                    = new Vector3(
                        obj.transform.position.x,
                        Random.Range(minSpawn.position.y, maxSpawn.position.y),
                        Random.Range(maxSpawn.position.z, minSpawn.position.z)
                    );
            }
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
        }

        public float yDraggingHeight = 2.1f;

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
                        Debug.Log("Nice");
                    }
                }
            }
        }

        private void RayCastInjectRang()
        {
            var ray = interactionCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray, 100f);

            foreach (var hit in hits)
            {
                if (hit.transform != null && hit.transform.CompareTag("PatientRang"))
                {
                    if (missingTooth.Contains(hit.transform.gameObject))
                    {
                        controlRang.gameObject.SetActive(false);
                        controlRang = null;

                        var tooth = patientTooth.Find(o => o.gameObject == hit.transform.gameObject);
                        tooth.enabled = true;
                        missingTooth.Remove(hit.transform.gameObject);
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

        private IEnumerator PlayCinematicPatientComing()
        {
            yield return StartCoroutine(patientMoveController.StartMoveFromFirst());
            yield return StartCoroutine(patientMoveController.RotatePatient(playerPosition.position));
            yield return StartCoroutine(patientMoveController.SitDownAndOpenMouth());
            yield return StartCoroutine(MovePlayerToward());

            firstPersonController.LockRotate();
            firstPersonController.ChangeCamera();

            ShowTable();
        }

        private IEnumerator MovePlayerToward()
        {
            yield return playerPosition.DOMove(playerEndPoint.position, 2f)
                .SetEase(Ease.Linear)
                .WaitForCompletion();
        }

        private IEnumerator MovePlayerBackWard()
        {
            yield return playerPosition.DOMove(playerStartPoint.position, 2f)
                .SetEase(Ease.Linear)
                .WaitForCompletion();
            ;
        }
    }
}