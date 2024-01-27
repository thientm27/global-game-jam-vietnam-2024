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

        // Pad rang
        [SerializeField] private Transform table;
        [SerializeField] private Transform maxSpawn;
        [SerializeField] private Transform minSpawn;
        [SerializeField] private GameObject rangModel;

        private void SpawnRang(int numberOfSpawn = 1)
        {
            for (int i = 0; i < numberOfSpawn; i++)
            {
                var obj = Instantiate(rangModel, table);
                obj.transform.position
                    = new Vector3(
                        Random.Range(minSpawn.position.x, maxSpawn.position.x),
                        obj.transform.position.y,
                        Random.Range(minSpawn.position.z, maxSpawn.position.z)
                    );
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
                patientTooth[i].enabled = false;
            }
        }

        void Start()
        {
            firstPersonController.LockMove();
            firstPersonController.LockJump();
            DisableRandomTooth(7);
            SpawnRang(7);
        }

        void Update()
        {
            firstPersonController.Controlling();
            if (Input.GetKeyDown(KeyCode.L))
            {
                StartCoroutine(PlayCinematicPatientComing());
            }
        }

        private IEnumerator PlayCinematicPatientComing()
        {
            yield return StartCoroutine(patientMoveController.StartMoveFromFirst());
            yield return StartCoroutine(patientMoveController.RotatePatient(playerPosition.position));
            yield return StartCoroutine(patientMoveController.SitDownAndOpenMouth());
            yield return StartCoroutine(MovePlayerToward());
            firstPersonController.LockRotate();
            firstPersonController.ChangeCameraRotateAt(patientMouth.position);
            firstPersonController.ChangeCameraView(30);
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