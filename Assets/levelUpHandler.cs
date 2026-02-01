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
    // 1. APLICAR LA MILLORA
    switch (upgrade.statToBuff)
    {
        case StatType.AttackDamage:
            player.playerDamage += upgrade.amount;
            // Si és permanent, sumem a l'extra per mantenir la coherència visual/lògica
            if (upgrade.isPermanent) player.playerDamageExtra += upgrade.amount;
            break;

        case StatType.MaxHealth:
            player.MaxHealth += upgrade.amount;
            player.health += upgrade.amount; // Curem el tros que hem pujat
            if (upgrade.isPermanent) player.maxHealthExtra += upgrade.amount;
            break;

        case StatType.Speed:
            player.speed += upgrade.amount;
            break;

        case StatType.Heal: 
            player.health = Mathf.Min(player.health + (player.MaxHealth * upgrade.amount), player.MaxHealth);
            break;

        case StatType.Cooldown:
            player.attackCooldown -= player.attackCooldown * upgrade.amount;
            break;
            
        // Si encara fas servir els tipus "Mask_", posa'ls aquí també apuntant a les mateixes variables
        case StatType.Mask_Damage:
            player.playerDamage += upgrade.amount;
            player.playerDamageExtra += upgrade.amount;
            break;
        case StatType.Mask_Speed:
            player.SpeedExtra += upgrade.amount;
            player.speed += upgrade.amount;
            break;
        case StatType.Mask_Shield:
            player.maxShieldExtra += upgrade.amount;
            player.shield += upgrade.amount;
            break;
        case StatType.Mask_Health:
            player.MaxHealth += upgrade.amount;
            player.maxHealthExtra += upgrade.amount;
            break;
    }

    // 2. GUARDAR PERMANENTMENT AL MOMENT (Si la millora és permanent)
    if (upgrade.isPermanent)
    {
        // IMPORTANT: Fem servir el mateix nom de clau que després llegirà 'EquipMask'
        // Si el teu StatType és 'AttackDamage', la clau serà "Mascara_0_AttackDamage"
        // Si és 'Mask_Damage', serà "Mascara_0_Mask_Damage"
        string clauUnica = "Mascara_" + player.currMask + "_" + upgrade.statToBuff.ToString();
        
        float valorActual = PlayerPrefs.GetFloat(clauUnica, 0f);
        PlayerPrefs.SetFloat(clauUnica, valorActual + upgrade.amount);
        PlayerPrefs.Save();
        
        Debug.Log($"Millora guardada al moment: {clauUnica} = {valorActual + upgrade.amount}");
    }

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