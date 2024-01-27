
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
            openMouth.SetActive(isOpen);
            closeMouth.SetActive(!isOpen);
        }
    }

 
}