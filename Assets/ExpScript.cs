using UnityEngine;

public class ExpScript : Collectible
{
    public int value = 1; 
    private bool jaRecollit = false;
    
    protected override void ApplyEffect(GameObject objectThatTouchedMe)
    {
        if (jaRecollit) return;
        jaRecollit = true;

        PlayerControl playerControl = objectThatTouchedMe.GetComponentInParent<PlayerControl>();
        
        // 1. Busquem el component AudioSource de la pròpia bola
        AudioSource elMeuAudio = GetComponent<AudioSource>();

        if (playerControl != null)
        {
            // 2. Lògica d'experiència
            playerControl.exp += value;
            playerControl.Exp_Gained();

            // 3. REPRODUCCIÓ DEL SO
            if (elMeuAudio != null && elMeuAudio.clip != null)
            {
                // Fem servir PlayClipAtPoint amb el clip que JA hi ha a l'AudioSource
                AudioSource.PlayClipAtPoint(elMeuAudio.clip, Camera.main.transform.position);
            }
            

            // 4. NETEJA I DESTRUCCIÓ
            // Amaguem la bola de seguida
            if (GetComponent<SpriteRenderer>()) GetComponent<SpriteRenderer>().enabled = false;
            if (GetComponent<Collider2D>()) GetComponent<Collider2D>().enabled = false;

            // Destruïm l'objecte (el so seguirà sonant gràcies a PlayClipAtPoint)
            Destroy(gameObject); 
        }
    }
}