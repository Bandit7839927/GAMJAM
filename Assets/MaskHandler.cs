using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class MaskHandler : MonoBehaviour
{

    public PlayerControl player; // Arrossega el teu jugador aquí
    public GameObject MaskPanel; // Arrossega el Panel de la UI
    public GameObject UI_player; // La UI normal del jugador
    public Image slotInventoriUI; // La UI normal del jugador
    public Image mascara_UI;    

    public MaskButton button1; // Script que farem ara (veure pas 4)
    public MaskButton button2;
    public MaskButton button3;

    public List<MaskData> possibleMask;

    void Start()
    {
        // Busquem el jugador si no està assignat
        if (player == null) player = FindObjectOfType<PlayerControl>();

        if (MaskPanel != null) 
        {
            MaskPanel.SetActive(true); // MOSTRAR la pantalla de selecció
            Time.timeScale = 0f;       // PAUSAR el joc fins que triïn
            if (slotInventoriUI == null)
            {
                // Busquem l'objecte pel seu nom exacte a la jerarquia
                GameObject UIp = GameObject.Find("UI"); 

                if (UIp != null)
                {
                    UI_player = UIp;
                }
                else
                {
                    Debug.LogError("No s'ha trobat cap objecte anomenat 'UI'!");
                }
            }
            show_update_mask();        // Carregar les dades als botons
        }
    }

    public void show_update_mask(){
        UI_player.SetActive(false);
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
        // 1. Apliquem la icona a la UI
        if (mascara_UI != null) mascara_UI.sprite = upgrade.menuIcon;

        // 2. Cridem a una funció nova al PlayerControl per gestionar el canvi
        // En lloc de sumar variables aquí, deixem que el Player ho gestioni tot
        player.EquipMask(upgrade); 

        CloseMenu();
    }

    void CloseMenu()
    {
        MaskPanel.SetActive(false);
        UI_player.SetActive(true);
        Time.timeScale = 1f; // TORNEM A ARRENCAR EL JOC
    }
}