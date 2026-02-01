using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class MaskHandler : MonoBehaviour
{

    public PlayerControl player; // Arrossega el teu jugador aquí
    public GameObject MaskPanel; // Arrossega el Panel de la UI
    public Image mascara_UI;    

    public MaskButton button1; // Script que farem ara (veure pas 4)
    public MaskButton button2;
    public MaskButton button3;

    public List<MaskData> possibleMask;

    void Start()
    {
        MaskPanel.SetActive(true); // Amaguem la pantalla al començar
    }

    public void show_update_mask(){
        // 1. PAUSEM EL JOC
        Time.timeScale = 0f; 
        MaskPanel.SetActive(true);

        // 1. Triem el primer
        MaskData choice1 = possibleMask[0];

        // 2. Triem el segon (assegurant que no és igual al primer)
        MaskData choice2 = possibleMask[1];

        // 3. Triem el tercer (assegurant que no és igual ni al 1 ni al 2)
        MaskData choice3 = possibleMask[2];

        // Configurem els 3 botons
        button1.Setup(choice1, this);
        button2.Setup(choice2, this);
        button3.Setup(choice3, this);
    }

    public void ApplyUpgrade_Mask(MaskData upgrade)
    {
        // APLICAR LA MILLORA AL JUGADOR

        mascara_UI.sprite = upgrade.menuIcon;
        player.currMask = upgrade.idMask; 
        player.health += upgrade.bonusHealth;
        // Si tens barra de vida, actualitza-la aquí
        player.speed += upgrade.bonusSpeed;
        // Assegura't de tenir aquesta variable al PlayerControl
        // player.attackDamage += upgrade.amount;
        player.playerDamage += upgrade.bonusDamage;
        player.shield += upgrade.bonusShield;

        CloseMenu();
    }

    void CloseMenu()
    {
        MaskPanel.SetActive(false);
        Time.timeScale = 1f; // TORNEM A ARRENCAR EL JOC
    }
}