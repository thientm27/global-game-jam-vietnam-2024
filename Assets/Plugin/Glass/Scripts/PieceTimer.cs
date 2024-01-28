using UnityEngine;

public class PieceTimer : MonoBehaviour
{
    [Range(0, 500)]
    public float timer;

    private void FixedUpdate()
    {
        if (timer > 0)
            timer -= Time.deltaTime;
        else
        {
            foreach (Transform child in transform)
                Destroy(child.gameObject);
            Destroy(gameObject);
        }    
            
    }
}
