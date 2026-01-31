using UnityEngine;

public abstract class Collectible : MonoBehaviour
{
    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        // Detectem si és el jugador o la seva hitbox
        if (other.CompareTag("Player") || other.CompareTag("PlayerHitbox"))
        {
            ApplyEffect(other.gameObject);
            // HEM ESBORRAT EL DESTROY D'AQUÍ!
        }
    }

    protected abstract void ApplyEffect(GameObject player);
}