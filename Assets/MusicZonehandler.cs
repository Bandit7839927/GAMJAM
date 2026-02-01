using UnityEngine;
using System.Collections;

public class MusicZoneHandler : MonoBehaviour
{
    [Header("Configuració d'Àudio")]
    public AudioSource cameraMusicSource; // Arrossega aquí la Main Camera (o l'objecte amb l'AudioSource)
    public float fadeSpeed = 1.5f;        // Velocitat del canvi de volum

    [Header("Límits de Zona")]
    public float changeXValue = 100f;     // Punt X on canvia la música

    [Header("Clips de Música")]
    public AudioClip musicZonaA;          // Música per a X < changeXValue
    public AudioClip musicZonaB;          // Música per a X > changeXValue

    private bool isPastX = false;
    private Transform playerTransform;
    private float targetVolume;

    void Start()
    {
        // Si no has arrossegat la càmera, la busquem
        if (cameraMusicSource == null)
            cameraMusicSource = Camera.main.GetComponent<AudioSource>();

        // Busquem el jugador pel Tag
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerTransform = player.transform;

        // Guardem el volum original que hagis posat a Unity
        targetVolume = cameraMusicSource.volume;

        // Inicialitzem la música segons on comenci el jugador
        if (playerTransform != null)
        {
            isPastX = playerTransform.position.x > changeXValue;
            cameraMusicSource.clip = isPastX ? musicZonaB : musicZonaA;
            cameraMusicSource.Play();
        }
    }

    void Update()
    {
        if (playerTransform == null) return;

        float playerX = playerTransform.position.x;

        // CAS A: Creuem cap a la zona B (Dreta)
        if (playerX > changeXValue && !isPastX)
        {
            isPastX = true;
            StopAllCoroutines();
            StartCoroutine(FadeToNewClip(musicZonaB));
        }
        // CAS B: Tornem cap a la zona A (Esquerra)
        else if (playerX < changeXValue && isPastX)
        {
            isPastX = false;
            StopAllCoroutines();
            StartCoroutine(FadeToNewClip(musicZonaA));
        }
    }

    private IEnumerator FadeToNewClip(AudioClip newClip)
    {
        // 1. Baixem el volum gradualment
        while (cameraMusicSource.volume > 0.01f)
        {
            cameraMusicSource.volume -= Time.deltaTime * fadeSpeed;
            yield return null;
        }

        cameraMusicSource.volume = 0;
        cameraMusicSource.Stop();

        // 2. Canviem el clip i tornem a arrencar
        cameraMusicSource.clip = newClip;
        cameraMusicSource.Play();

        // 3. Pugem el volum gradualment fins al volum original
        while (cameraMusicSource.volume < targetVolume)
        {
            cameraMusicSource.volume += Time.deltaTime * fadeSpeed;
            yield return null;
        }
        
        cameraMusicSource.volume = targetVolume;
    }

    // Visualització al Editor per saber on està la línia de canvi
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Vector3 linePos = new Vector3(changeXValue, 0, 0);
        Gizmos.DrawLine(linePos + Vector3.up * 50, linePos + Vector3.down * 50);
    }
}