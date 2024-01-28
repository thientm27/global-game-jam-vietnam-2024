using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameScene.Component;
using UnityEngine;
using UnityEngine.Events;

namespace GameScene
{
    public class PatientMoveController : MonoBehaviour
    {
        [SerializeField] private List<Transform> movePoint;
        [SerializeField] private Patient patient;
        [SerializeField] private float patientMoveSpeed = 3f;
        [SerializeField] private string walkTrigger;
        [SerializeField] private string sitTrigger;
        [SerializeField] private string payTrigger;
        [SerializeField] private string hitTrigger;
        public Patient Patient
        {
            get => patient;
            set => patient = value;
        }

        public void OpenMouse(bool isOpen)
        {
            if (isOpen)
            {
                SetAnimation(PatientAnimationType.Sitting);
            }

            patient.OpenMouth(isOpen);
        }

        public IEnumerator StandUp()
        {
            SetAnimation(PatientAnimationType.Idle);
            yield return new WaitForSeconds(2f);
        }

        public IEnumerator SitDown()
        {
            SetAnimation(PatientAnimationType.Sitting);
            yield return new WaitForSeconds(2f);
        }

        public IEnumerator Pay()
        {
            SetAnimation(PatientAnimationType.Pay);
            yield return new WaitForSeconds(2f);
        }

        public void SetAnimation(PatientAnimationType animationType)
        {
            patient.Animator.SetBool(hitTrigger, false);
            patient.Animator.SetBool(payTrigger, false);
            patient.Animator.SetBool(walkTrigger, false);
            patient.Animator.SetBool(sitTrigger, false);
            switch (animationType)
            {
                case PatientAnimationType.Idle:
                    patient.Animator.SetBool(hitTrigger, false);
                    patient.Animator.SetBool(payTrigger, false);
                    patient.Animator.SetBool(walkTrigger, false);
                    patient.Animator.SetBool(sitTrigger, false);
                    break;
                case PatientAnimationType.Move:
                    patient.Animator.SetBool(walkTrigger, true);
                    break;
                case PatientAnimationType.Sitting:
                    patient.Animator.SetBool(sitTrigger, true);
                    break;
                case PatientAnimationType.Pay:
                    patient.Animator.SetBool(payTrigger, true);
                    break;
                case PatientAnimationType.GotHit:
                    patient.Animator.SetBool(hitTrigger, true);
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

        public IEnumerator StartMoveFromEnd()
        {
            SetAnimation(PatientAnimationType.Move);
            Stack<Transform> stack = new Stack<Transform>();

            foreach (var obj in movePoint)
            {
                stack.Push(obj);
            }

            while (stack.Count > 0)
            {
                var pointToMove = stack.Pop();
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
        Sitting,
        Pay,
        GotHit
    }
}