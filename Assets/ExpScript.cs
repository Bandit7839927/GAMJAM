using UnityEngine;

public class ExpScript : Collectible
{
    public int value = 1; 

    protected override void ApplyEffect(GameObject player)
    {
        PlayerControl playerControl = player.GetComponent<PlayerControl>();
        
        if (playerControl != null){
            playerControl.exp += value;
            Debug.Log("¡EXP añadida! Total en el jugador: " + playerControl.exp);
        }
    }
}