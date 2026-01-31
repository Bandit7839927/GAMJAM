using UnityEngine;

public class ExpScript : Collectible
{
    public int value = 1; 

    protected override void ApplyEffect(GameObject objectThatTouchedMe)
    {
        // BUSCAMOS EN EL PADRE: Esto soluciona el problema del hijo colisionador
        PlayerControl playerControl = objectThatTouchedMe.GetComponentInParent<PlayerControl>();

        if (playerControl != null)
        {
            playerControl.exp += value;
            Debug.Log("¡EXP añadida! Total en el jugador: " + playerControl.exp);
            playerControl.Exp_Gained();
        }
        else 
        {
            // Si ves esto en la consola, el script PlayerControl no está en el padre
            Debug.LogWarning("No se encontró PlayerControl en el padre de: " + objectThatTouchedMe.name);
        }
    }
}