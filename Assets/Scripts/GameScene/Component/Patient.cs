using UnityEngine;

namespace GameScene.Component
{
    public class Patient : MonoBehaviour
    {
        [SerializeField] private GameObject openMouth;
        [SerializeField] private GameObject closeMouth;
        [SerializeField] private Animator animator;
        public Animator Animator => animator;


        public void OpenMouth(bool isOpen = true)
        {
            if (isOpen)
            {
                openMouth.transform.position = closeMouth.transform.position;
                closeMouth.transform.position += Vector3.up * 100;
            }
            else
            {
                closeMouth.transform.position = openMouth.transform.position;
                openMouth.transform.position += Vector3.up * 100;
            }
        }
    }
}