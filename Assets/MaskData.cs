using UnityEngine;

[CreateAssetMenu(fileName = "NovaMascara", menuName = "Roguelike/Mascara")]
public class MaskData : ScriptableObject
{
    [Header("Identificació")]
    public int idMask; // Ex: 0 -> Defauly, 1->Mexico
    public string maskName;
    [TextArea] public string description;

    [Header("Visuals")]
    public Sprite menuIcon; // La foto pel menú de selecció
    public Sprite inGameSprite; // La màscara que es veurà posada al personatge

    [Header("Poders")]
    public float baseHealth;
    public float baseDamage;
    public float baseSpeed;
    public float baseShield;
}