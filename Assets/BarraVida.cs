using UnityEngine;
using UnityEngine.UI;

public class BarraVida : MonoBehaviour
{
    public Image BarraDeVida;
    private PlayerControl PlayerControler;

    void Start()
    {
        // Busquem el jugador
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            PlayerControler = playerObj.GetComponent<PlayerControl>();
        }
    }

    void Update()
    {
        if (PlayerControler != null && BarraDeVida != null)
        {
            // IMPORTANT: Llegim sempre la MaxHealth actual del Player
            // Així, si la màscara dona més vida, la barra ho sabrà.
            float percentatge = PlayerControler.health / PlayerControler.MaxHealth;

            // Ens assegurem que el valor estigui entre 0 i 1 per evitar errors visuals
            BarraDeVida.fillAmount = Mathf.Clamp01(percentatge);
        }
    }
}