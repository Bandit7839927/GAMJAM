using UnityEngine;

public class ObjecteRecollible : MonoBehaviour
{
    [Header("Configuració")]
    [Tooltip("El prefab que anirà a l'inventari (ampolla_item)")]
    public GameObject prefabDeLobjecte; 
    
    [Tooltip("L'objecte fill que s'encén/apaga (Square)")]
    public GameObject iconaVisual; 

    // OnValidate s'executa automàticament a l'Editor de Unity
    private void OnValidate()
    {
        // 1. Si no hem posat l'icona a mà, la busquem per nom
        if (iconaVisual == null)
        {
            Transform t = transform.Find("Square");
            if (t != null) iconaVisual = t.gameObject;
        }

        // 2. Si no hem posat el prefab, mirem si el trobem a Resources
        if (prefabDeLobjecte == null)
        {
            prefabDeLobjecte = Resources.Load<GameObject>("ampolla_item");
        }
    }

    void Start()
    {
        // Ens assegurem que la icona estigui apagada al començar
        if (iconaVisual != null) iconaVisual.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerControl player = other.GetComponent<PlayerControl>();
        if (player == null) player = other.GetComponentInParent<PlayerControl>();

        if (player != null)
        {
            player.objecteAprop = this;
            if (iconaVisual != null) iconaVisual.SetActive(true);
            Debug.Log("Pots agafar: " + gameObject.name);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerControl player = other.GetComponent<PlayerControl>();
        if (player == null) player = other.GetComponentInParent<PlayerControl>();

        if (player != null && player.objecteAprop == this)
        {
            player.objecteAprop = null;
            if (iconaVisual != null) iconaVisual.SetActive(false);
        }
    }
}