using UnityEngine;

public class ObjecteRecollible : MonoBehaviour
{
    [Header("Configuració")]
    public GameObject prefabDeLobjecte; 

    // --- NOU: Arrossega aquí l'objecte FILL que té l'Sprite de la N ---
    public GameObject iconaVisual; 

    void Start()
    {
        // 1. Al començar, apaguem l'icona perquè no es vegi d'inici
        if (iconaVisual != null)
        {
            iconaVisual.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerControl player = other.GetComponent<PlayerControl>();

        if (player != null)
        {
            player.objecteAprop = this;

            // 2. Quan entres, ENCENEM l'icona
            if (iconaVisual != null) iconaVisual.SetActive(true);

            Debug.Log("Pots agafar: " + gameObject.name);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        PlayerControl player = other.GetComponent<PlayerControl>();

        if (player != null)
        {
            if (player.objecteAprop == this)
            {
                player.objecteAprop = null;

                // 3. Quan surts, APAGUEM l'icona
                if (iconaVisual != null) iconaVisual.SetActive(false);

                Debug.Log("T'has allunyat.");
            }
        }
    }
}