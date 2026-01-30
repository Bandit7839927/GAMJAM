using UnityEngine;


public abstract class Collectible : MonoBehaviour
{
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.CompareTag("PlayerHitbox"))
        {
            ApplyEffect(other.gameObject); 
            Destroy(gameObject);          
        }
    }

    
    protected abstract void ApplyEffect(GameObject player);
}