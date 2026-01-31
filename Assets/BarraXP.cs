using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BarraXP : MonoBehaviour
{
    public Image BarraDeXp;
    private PlayerControl PlayerControler;
    private float Xp_maxima;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PlayerControler = GameObject.Find("Player").GetComponent<PlayerControl>();
        Xp_maxima = PlayerControler.xp_lvl[PlayerControler.level];

    }

    // Update is called once per frame
    void Update()
    {
        Xp_maxima = PlayerControler.xp_lvl[PlayerControler.level];
        if(PlayerControler.level > 0) BarraDeXp.fillAmount = (PlayerControler.exp - PlayerControler.xp_lvl[PlayerControler.level -1]) / Xp_maxima;
        else BarraDeXp.fillAmount = (PlayerControler.exp) / Xp_maxima;
    }

        
}
