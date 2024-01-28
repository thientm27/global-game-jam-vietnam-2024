using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GameScene.Component
{
    public class GlassTrigger : MonoBehaviour
    {
        [SerializeField] private UnityEvent glassSound;
        [SerializeField] private List<Rigidbody> glassPart;

        private void OnTriggerEnter(Collider other)
        {
            if (other.transform.CompareTag("Player"))
            {
                Debug.Log("Glassing");
                glassSound.Invoke();
                foreach (var o in glassPart)
                {
                    o.isKinematic = false;
                }
            }
        }
    }
}