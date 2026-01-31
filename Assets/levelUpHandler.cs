using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class levelUpHandler : MonoBehaviour
{
    public PlayerControl player; // Arrossega el teu jugador aquí
    public GameObject levelUpPanel; // Arrossega el Panel de la UI

    public UpgradeButton button1; // Script que farem ara (veure pas 4)
    public UpgradeButton button2;
    public UpgradeButton button3;

    public List<UpgradeData> possibleUpgrades;

    void Start()
    {
        levelUpPanel.SetActive(false); // Amaguem la pantalla al començar
    }

    public void show_update(){
        // 1. PAUSEM EL JOC
        Time.timeScale = 0f; 
        levelUpPanel.SetActive(true);

        // 1. Triem el primer
        UpgradeData choice1 = possibleUpgrades[Random.Range(0, possibleUpgrades.Count)];

        // 2. Triem el segon (assegurant que no és igual al primer)
        UpgradeData choice2 = possibleUpgrades[Random.Range(0, possibleUpgrades.Count)];
        while (choice2 == choice1)
        {
            choice2 = possibleUpgrades[Random.Range(0, possibleUpgrades.Count)];
        }

        // 3. Triem el tercer (assegurant que no és igual ni al 1 ni al 2)
        UpgradeData choice3 = possibleUpgrades[Random.Range(0, possibleUpgrades.Count)];
        while (choice3 == choice1 || choice3 == choice2)
        {
            choice3 = possibleUpgrades[Random.Range(0, possibleUpgrades.Count)];
        }

        // Configurem els 3 botons
        button1.Setup(choice1, this);
        button2.Setup(choice2, this);
        button3.Setup(choice3, this);
    }

    public void ApplyUpgrade(UpgradeData upgrade)
    {
        // APLICAR LA MILLORA AL JUGADOR
        switch (upgrade.statToBuff)
        {
            case StatType.MaxHealth:
                player.health += upgrade.amount;
                // Si tens barra de vida, actualitza-la aquí
                break;
            case StatType.Speed:
                player.speed += upgrade.amount;
                break;
            case StatType.AttackDamage:
                // Assegura't de tenir aquesta variable al PlayerControl
                // player.attackDamage += upgrade.amount;
                Debug.Log("Dany pujat!");
                break;
        }

        // Si és permanent, la guardem (Exemple bàsic)
        if (upgrade.isPermanent)
        {
            PlayerPrefs.SetFloat(upgrade.statToBuff.ToString(), upgrade.amount);
            PlayerPrefs.Save();
        }

        CloseMenu();
    }

    void CloseMenu()
    {
        levelUpPanel.SetActive(false);
        Time.timeScale = 1f; // TORNEM A ARRENCAR EL JOC
    }
}
