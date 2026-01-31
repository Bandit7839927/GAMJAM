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

        if (playerControl != null)
        {
            // 1. Lògica d'experiència
            playerControl.exp += value;
            playerControl.Exp_Gained();

            // 2. ESBORRAR EL VISUAL I L'ANIMACIÓ IMMEDIATAMENT
            // Desactivem el SpriteRenderer (el dibuix)
            if (GetComponent<SpriteRenderer>()) GetComponent<SpriteRenderer>().enabled = false;
            
            // Desactivem l'Animator (l'animació)
            if (GetComponent<Animator>()) GetComponent<Animator>().enabled = false;

            // 3. ESBORRAR L'OBJECTE SENCER (inclou el script Collectible)
            // Fem servir 'this.gameObject' per assegurar que matem el contenidor
            Destroy(this.gameObject); 
            
            Debug.Log("S'ha esborrat el Sprite, l'Animació i el GameObject.");
        }
    }
}