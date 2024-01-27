using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameScene.Component;
using UnityEngine;

namespace GameScene
{
    public class PatientMoveController : MonoBehaviour
    {
        [SerializeField] private List<Transform> movePoint;
        [SerializeField] private Patient patient;
        [SerializeField] private float patientMoveSpeed = 3f;
        [SerializeField] private string walkTrigger;
        [SerializeField] private string sitTrigger;
        public Patient Patient
        {
            get => patient;
            set => patient = value;
        }

        public IEnumerator SitDownAndOpenMouth()
        {
            SetAnimation(PatientAnimationType.Sitting);
            yield return new WaitForSeconds(2f);
            patient.OpenMouth(true);
        }

        public void SetAnimation(PatientAnimationType animationType)
        {
            patient.Animator.SetBool(walkTrigger, false);
            patient.Animator.SetBool(sitTrigger, false);
            switch (animationType)
            {
                case PatientAnimationType.Idle:
                    patient.Animator.SetBool(walkTrigger, false);
                    patient.Animator.SetBool(sitTrigger, false);
                    break;
                case PatientAnimationType.Move:
                    patient.Animator.SetBool(walkTrigger, true);
                    break;
                case PatientAnimationType.Sitting:
                    patient.Animator.SetBool(sitTrigger, true);
                    break;
            }
        }

        public IEnumerator StartMoveFromFirst()
        {
            SetAnimation(PatientAnimationType.Move);
            Queue<Transform> queue = new Queue<Transform>();
            foreach (var obj in movePoint)
            {
                queue.Enqueue(obj);
            }

            while (queue.Count > 0)
            {
                var pointToMove = queue.Dequeue();
                yield return MovingPatient(pointToMove.position);
            }

            SetAnimation(PatientAnimationType.Idle);

        }

        public IEnumerator RotatePatient(Vector3 targetMove)
        {
            yield return patient.transform.DOLookAt(targetMove, 1f).SetEase(Ease.Linear)
                .WaitForCompletion();

           
        }

        private IEnumerator MovingPatient(Vector3 targetMove)
        {
            patient.transform.DOLookAt(targetMove, 1f).SetEase(Ease.Linear);

            yield return patient.transform.DOMove(targetMove,
                    Vector3.Distance(patient.transform.position, targetMove) / patientMoveSpeed
                ).SetEase(Ease.Linear)
                .WaitForCompletion();
        }
    }

    public enum PatientAnimationType
    {
        Idle,
        Move,
        Sitting
    }
}