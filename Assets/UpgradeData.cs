using UnityEngine;

// Això permet fer clic dret a Unity i crear la millora
[CreateAssetMenu(fileName = "NovaMillora", menuName = "Roguelike/Millora")]
public class UpgradeData : ScriptableObject
{
    [Header("Info Visual")]
    public string upgradeName;      // Títol (ex: "Més Vida")
    [TextArea] 
    public string description;      // Descripció (ex: "Et cura 20 punts")
    public Sprite icon;             // Imatge de la carta
    public Color backgroundColor; // Color de fons de la carta

    [Header("Efectes")]
    public StatType statToBuff;     // Quina estadística millora?
    public float amount;            // Quant puja?
    public bool isPermanent;        // Es guarda per sempre?
}

// LLISTA DE TIPUS D'ESTADÍSTIQUES
public enum StatType 
{ 
    MaxHealth, 
    Speed, 
    AttackDamage,
    Cooldown,
    shield,
    attackSpeed
}