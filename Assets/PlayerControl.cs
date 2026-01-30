using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    public float speed = 5f;
    public float health = 100f;
    public float shield = 50f;
    public float attackcooldown = 1f;
    public float attackdamage = 10f;

    enum playerstate { idle, running, dead, attacking, parrying}
    playerstate currentstate = playerstate.idle;    


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start(){
        
    }

    void movementhandler(){
         //movement input handling
        var keyboard = Keyboard.current;
        float moveX = 0f;
        if (keyboard.aKey.isPressed) moveX = -1f;
        if (keyboard.dKey.isPressed) moveX = 1f;
        float moveY = 0f;
        if (keyboard.wKey.isPressed) moveY = 1f;
        if (keyboard.sKey.isPressed) moveY = -1f;

        Vector3 movement = new Vector3(moveX, moveY, 0).normalized * speed * Time.deltaTime;
        transform.Translate(movement);
    }

    // Update is called once per frame
    void FixedUpdate(){

        movementhandler();




    }
}
