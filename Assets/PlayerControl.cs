using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControl : MonoBehaviour
{
    public int level = 1;
    public int exp = 0;
    public float speed = 5f;
    public float health = 100f;
    public float shield = 50f;
    public float attackcooldown = 1f;
    public float attackdamage = 10f;

    public bool lookingLeft;

    enum playerstate { idle, running, dead, attacking, parrying}
    playerstate currentstate = playerstate.idle;    

    void Start(){
        lookingLeft = false;
    }

    void movementhandler(){
        var keyboard = Keyboard.current;
        if (keyboard == null) return; // Seguridad por si no hay teclado conectado

        float moveX = 0f;
        if (keyboard.aKey.isPressed) moveX = -1f;
        if (keyboard.dKey.isPressed) moveX = 1f;

        float moveY = 0f;
        if (keyboard.wKey.isPressed) moveY = 1f;
        if (keyboard.sKey.isPressed) moveY = -1f;

        // --- LÓGICA DE FLIP ---
        if (moveX < 0 && !lookingLeft) {
            Flip();
        }
        else if (moveX > 0 && lookingLeft) {
            Flip();
        }

        // Movimiento (Nota: En Beat 'em up usamos Z para profundidad, pero si prefieres Y está bien)
        Vector3 movement = new Vector3(moveX, moveY, 0).normalized * speed * Time.deltaTime;
        transform.Translate(movement);
    }
    void AddExp(int amount) {
        exp += amount;
        if(exp >= level * 10){
            ++level;
            exp = 0;
            health = 100f; 
            Debug.Log("¡Nivel aumentado! Nuevo nivel: " + level);
        }
        
    }
    void Flip() {
        lookingLeft = !lookingLeft;
        
        // Multiplicamos la escala X por -1 para girar el sprite y todos sus hijos
        Vector3 currentScale = transform.localScale;
        currentScale.x *= -1;
        transform.localScale = currentScale;
    }

    void Update() {
        // El input de teclado es mejor procesarlo en Update para que no se pierdan pulsaciones
        movementhandler();
    }
}