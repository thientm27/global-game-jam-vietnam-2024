using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GameScene.Component
{
    public class GlassTrigger : MonoBehaviour
    {
        [SerializeField] private UnityEvent glassSound;
        [SerializeField] private GameObject origin;
        [SerializeField] private List<Rigidbody> glassPart;

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.CompareTag("Player"))
            {
                Debug.Log("Glassing");
                origin.SetActive(false);
                glassSound.Invoke();
                foreach (var o in glassPart)
                {
                    o.gameObject.SetActive(true);
                    o.isKinematic = false;
                }
            }
        }
    }
}