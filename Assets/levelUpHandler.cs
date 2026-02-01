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
    [Header("Audio")]
    public AudioSource lvlUp;
    void Start()
    {
        levelUpPanel.SetActive(false); // Amaguem la pantalla al començar
    }

    public void show_update()
    {
        
        if (lvlUp != null)
            lvlUp.Play();

        // ⏸ PAUSE GAME
        Time.timeScale = 0f;
        levelUpPanel.SetActive(true);

        // ---- rest stays the same ----
        UpgradeData choice1 = possibleUpgrades[Random.Range(0, possibleUpgrades.Count)];

        UpgradeData choice2 = possibleUpgrades[Random.Range(0, possibleUpgrades.Count)];
        while (choice2 == choice1)
            choice2 = possibleUpgrades[Random.Range(0, possibleUpgrades.Count)];

        UpgradeData choice3 = possibleUpgrades[Random.Range(0, possibleUpgrades.Count)];
        while (choice3 == choice1 || choice3 == choice2)
            choice3 = possibleUpgrades[Random.Range(0, possibleUpgrades.Count)];

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
                player.playerDamage += upgrade.amount;
                break;
            case StatType.Cooldown:
                // Assegura't de tenir aquesta variable al PlayerControl
                // player.attackDamage += upgrade.amount;
                player.attackCooldown += upgrade.amount;
                break;
            case StatType.shield:
                // Assegura't de tenir aquesta variable al PlayerControl
                // player.attackDamage += upgrade.amount;
                player.shield += upgrade.amount;
                break;
            case StatType.attackSpeed:
                // Assegura't de tenir aquesta variable al PlayerControl
                // player.attackDamage += upgrade.amount;
                player.attackDuration += upgrade.amount;
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
