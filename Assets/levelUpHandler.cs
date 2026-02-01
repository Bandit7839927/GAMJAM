using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class levelUpHandler : MonoBehaviour
{
    public PlayerControl player;
    public GameObject levelUpPanel;

    public UpgradeButton button1;
    public UpgradeButton button2;
    public UpgradeButton button3;

    public List<UpgradeData> possibleUpgrades;

    // COMPTADOR DE CUA
    private int nivellsPendents = 0; 

    [Header("Audio")]
    public AudioSource lvlUp;

    void Start()
    {
        levelUpPanel.SetActive(false);
    }

    // Aquesta funció la crida el PlayerControl
    public void show_update()
    {
        nivellsPendents++; // Afegim un nivell a la llista d'espera
        
        // Si el panell ja està obert, no fem res (la cua ja ha pujat)
        if (levelUpPanel.activeSelf) return;

        ObrirProximMenu();
    }

    // Nova funció interna per carregar dades
    private void ObrirProximMenu()
    {
        if (lvlUp != null) lvlUp.Play();

        // Pausa i mostra
        Time.timeScale = 0f;
        levelUpPanel.SetActive(true);

        // Triar millores aleatòries (el teu codi original)
        UpgradeData choice1 = possibleUpgrades[Random.Range(0, possibleUpgrades.Count)];

        UpgradeData choice2 = possibleUpgrades[Random.Range(0, possibleUpgrades.Count)];
        while (choice2 == choice1)
            choice2 = possibleUpgrades[Random.Range(0, possibleUpgrades.Count)];

        UpgradeData choice3 = possibleUpgrades[Random.Range(0, possibleUpgrades.Count)];
        while (choice3 == choice1 || choice3 == choice2)
            choice3 = possibleUpgrades[Random.Range(0, possibleUpgrades.Count)];

        // Enviar dades als teus UpgradeButtons
        button1.Setup(choice1, this);
        button2.Setup(choice2, this);
        button3.Setup(choice3, this);
    }

    public void ApplyUpgrade(UpgradeData upgrade)
    {
        // 1. APLICAR LA MILLORA (El teu switch original de stats)
        switch (upgrade.statToBuff)
        {
            case StatType.MaxHealth: player.MaxHealth += upgrade.amount; break;
            case StatType.Speed: player.speed += upgrade.amount; break;
            case StatType.AttackDamage: player.playerDamage += upgrade.amount; break;
            case StatType.shield: player.shield += upgrade.amount; break;
            case StatType.Cooldown: player.attackCooldown -= player.attackCooldown * upgrade.amount; break;
            case StatType.Heal: 
                player.health += player.MaxHealth * upgrade.amount;
                if (player.health > player.MaxHealth) player.health = player.MaxHealth;
                break;
        }

        if (upgrade.isPermanent)
        {
            PlayerPrefs.SetFloat(upgrade.statToBuff.ToString(), upgrade.amount);
            PlayerPrefs.Save();
        }

        // 2. CRIDAR AL TANCAMENT LÒGIC
        CloseMenu();
    }

    void CloseMenu()
    {
        nivellsPendents--; // Restem un de la cua perquè ja l'hem escollit

        if (nivellsPendents > 0)
        {
            // Si encara queden nivells, "refresquem" el menú amb noves cartes
            // No posem Time.timeScale a 1 ni amaguem el panell realment
            ObrirProximMenu();
        }
        else
        {
            // Si la cua està buida, tanquem de veritat
            levelUpPanel.SetActive(false);
            Time.timeScale = 1f; 
        }
    }
}