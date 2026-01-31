using UnityEngine;

public class HitboxDamage : MonoBehaviour
{
    public int damageAmount = 10;

    private void OnTriggerEnter2D(Collider2D other)
    {
        // --- PROTECCIÓ CRÍTICA ---
        // Si l'objecte que toquem té el mateix "pare" que jo (el puny), sóc jo mateix!
        // Aquesta comprovació és més segura que els Tags.
        if (other.gameObject == transform.parent.gameObject) return;
        
        // Alternativa si el puny no és fill directe: 
        if (other.CompareTag("Player")) return;
        // -------------------------

        if (other.CompareTag("Enemy"))
        {
            // Codi per fer mal a l'enemic
            Debug.Log("Puny ha tocat enemic!");
        }
    }
}