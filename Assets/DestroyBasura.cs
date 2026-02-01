using UnityEngine;

public class DestroyBasura : MonoBehaviour
{
    public GameObject efectoParticulas;
    public Sprite texturaDestruida;
    public int xpQueDona = 1; 

    private bool jaDestruit = false; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player_attack") && !jaDestruit)
        {
            jaDestruit = true; 

            // --- PROBABILITAT 1 DE CADA 3 ---
            // Random.Range(1, 4) genera un número entre 1, 2 i 3 (el 4 queda exclòs)
            int atzar = Random.Range(1, 4); 

            if (atzar == 1) // Només entra aquí el 33% de les vegades
            {
                PlayerControl jugador = FindFirstObjectByType<PlayerControl>();
                
                if (jugador != null)
                {
                    jugador.exp += xpQueDona;
                    jugador.Exp_Gained();
                    Debug.Log("Sort! Has guanyat XP. Total: " + jugador.exp);
                }
            }
            else 
            {
                Debug.Log("Aquesta vegada la brossa no tenia XP.");
            }

            // --- VISUALS I EXPLOSIÓ (Això passa sempre) ---
            SpriteRenderer sr = GetComponent<SpriteRenderer>();
            if (sr != null && texturaDestruida != null)
            {
                sr.sprite = texturaDestruida;
            }

            Explotar();
        }
    }

    void Explotar()
    {
        if (efectoParticulas != null)
        {
            GameObject particulas = Instantiate(efectoParticulas, transform.position, Quaternion.identity);
            ParticleSystem ps = particulas.GetComponent<ParticleSystem>();
            
            if (ps != null)
            {
                ps.Clear();
                ps.Play();
            }
        }

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Destroy(gameObject, 2f); 
    }
}