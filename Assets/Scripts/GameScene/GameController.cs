using System.Collections;
using GameScene.Component;
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

        void Start()
        {
            firstPersonController.LockMove();
            firstPersonController.LockJump();
        }

        void Update()
        {
            firstPersonController.Controlling();
            if (Input.GetKeyDown(KeyCode.L))
            {
                PlayCinematicPatientComing();
            }
        }

        private void PlayCinematicPatientComing()
        {
            StartCoroutine(patientMoveController.StartMoveFromFirst(() =>
            {
                StartCoroutine(patientMoveController.RotatePatient(playerPosition.position,
                    () => { StartCoroutine(patientMoveController.SitDownAndOpenMouth()); }));
            }));
        }
    }
}