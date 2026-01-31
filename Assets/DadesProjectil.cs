using UnityEngine;

public class DadesProjectil : MonoBehaviour
{
    [Header("Info per la UI")]
    public string nomItem = "Ampolla";
    public Sprite iconaUI; // <--- Aquesta és la imatge que es veurà a l'inventari!

    [Header("Estadístiques de Combat")]
    public float damage = 10f;
    public float velocitatLlançament = 10f; // Força amb la que surt disparat
}