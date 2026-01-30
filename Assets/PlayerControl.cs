using UnityEngine;
using UnityEngine.InputSystem; 
using System.Collections;

public class PlayerControl : MonoBehaviour
{
    [Header("Stats")]
    public float speed = 5f;
    public float health = 100f;
    public float shield = 50f;

    [Header("Atac")]
    public GameObject punt_atac;
    public float attackCooldown = 1f;
    public float attackDuration = 0.2f; // Duració màxima definida a l'inspector
    
    // Variables privades de control
    private float currentAttackTimer;   // Temporitzador intern
    private bool able_to_attack = true; // Control del Cooldown
    private bool lookingLeft = false;   // Control de gir

    enum playerstate { idle, running, dead, attacking, parrying }
    playerstate currentstate = playerstate.idle;    

    void Start()
    {
        if(punt_atac != null) punt_atac.SetActive(false);
        lookingLeft = false;
    }

    // --- FIX 1: Canviem 'var' per 'Keyboard' ---
    void movementhandler(Keyboard keyboard)
    {
        if (keyboard == null) return; 

        float moveX = 0f;
        if (keyboard.aKey.isPressed) moveX = -1f;
        if (keyboard.dKey.isPressed) moveX = 1f;

        float moveY = 0f;
        if (keyboard.wKey.isPressed) moveY = 1f;
        if (keyboard.sKey.isPressed) moveY = -1f;

        // --- LÒGICA DE FLIP ---
        if (moveX < 0 && !lookingLeft) {
            Flip();
        }
        else if (moveX > 0 && lookingLeft) {
            Flip();
        }

        Vector3 movement = new Vector3(moveX, moveY, 0).normalized * speed * Time.deltaTime;
        transform.Translate(movement);

        // Actualitzem estat visual si ens movem
        if (movement.magnitude > 0) currentstate = playerstate.running;
        else currentstate = playerstate.idle;
    }

    void Flip() 
    {
        lookingLeft = !lookingLeft;
        Vector3 currentScale = transform.localScale;
        currentScale.x *= -1;
        transform.localScale = currentScale;
    }

    void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        // --- PART 1: GESTIÓ DEL TEMPS D'ATAC ---
        // Si estem atacant, restem temps
        if (currentstate == playerstate.attacking)
        {
            currentAttackTimer -= Time.deltaTime;

            if (currentAttackTimer <= 0)
            {
                Debug.Log("El temps s'ha acabat! Cridant EndAttack()");
                EndAttack();
            }
            return; // IMPORTANT: Si ataquem, no fem res més (ni moure'ns)
        }

        // --- PART 2: INICIAR ATAC ---
        if (keyboard.mKey.wasPressedThisFrame && able_to_attack)
        {
            StartAttack();
            return;
        }

        // --- PART 3: MOVIMENT (Només si no ataquem) ---
        movementhandler(keyboard);
    }

    void StartAttack()
    {
        Debug.Log("ACTIVEM HITBOX");
        
        // 1. Canviem estat
        currentstate = playerstate.attacking;
        
        // 2. Bloquegem futurs atacs (Cooldown)
        able_to_attack = false;

        // 3. Reiniciem el temporitzador (Important!)
        currentAttackTimer = attackDuration;

        // 4. Activem la hitbox física
        if (punt_atac != null) punt_atac.SetActive(true);
    }

    void EndAttack()
    {
        Debug.Log("DESACTIVEM HITBOX");
        
        // 1. Tornem a estat normal (ja ens podem moure)
        currentstate = playerstate.idle;

        // 2. Desactivem la hitbox
        if (punt_atac != null) punt_atac.SetActive(false);

        // 3. Programem que el cooldown s'acabi d'aquí a X segons
        Invoke(nameof(ResetCooldown), attackCooldown);
    }

    void ResetCooldown()
    {
        able_to_attack = true;
        Debug.Log("Cooldown acabat, llest per atacar!");
    }
}