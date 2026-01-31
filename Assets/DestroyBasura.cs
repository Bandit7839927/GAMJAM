using UnityEngine;

public class DestroyBasura : MonoBehaviour
{
    // Aquí arrastraremos nuestro Prefab de partículas desde el Inspector
    public GameObject efectoParticulas;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player_attack"))
        {
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
                ps.Clear(); // Borra cualquier rastro de simulaciones viejas
                ps.Play();  // Inicia la explosión desde el segundo 0
            }
        }
        Destroy(gameObject);
    }
}