using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BarraVida : MonoBehaviour
{
    public Image BarraDeVida;
    private PlayerControl PlayerControler;
    private float vida_maxima;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayerControler = GameObject.Find("Player").GetComponent<PlayerControl>();
        vida_maxima = PlayerControler.MaxHealth;

    }

    // Update is called once per frame
    void Update()
    {
        BarraDeVida.fillAmount = PlayerControler.health / vida_maxima;
    }
}
