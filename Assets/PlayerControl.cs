using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerControl : MonoBehaviour
{
    [Header("Stats")]
    public float speed = 35f;
    public float health = 100f;
    public float MaxHealth = 100f;
    public float shield = 50f;
    public float playerDamage = 10f;

    [Header("Atac")]
    public GameObject punt_atac;
    public float attackCooldown = 0.1f;
    public float attackDuration = 0.2f;

    private float currentAttackTimer;
    private bool able_to_attack = true;
    private bool lookingLeft = false;

    enum playerstate { idle, running, dead, attacking, parrying }
    private Animator anim;

    playerstate currentstate = playerstate.idle;
    

    [Header("Nivells")]
    public int level = 0;
    public int exp = 0;
    public int[] xp_lvl = { 5, 10, 20, 30, 40 };
    public levelUpHandler levelManager;
    
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gameObject.tag = "Player";

        anim = GetComponent<Animator>();
        
        if (punt_atac != null) punt_atac.SetActive(false);

        // Configuración de Hitboxes e Hijos
        Transform hijoColisionador = transform.Find("ObjCollider");
        Transform hijoSquare = transform.Find("Square");
        rb.sleepMode = RigidbodySleepMode2D.NeverSleep;

        if (hijoSquare != null) {
            hijoSquare.gameObject.tag = "Player_attack";
            punt_atac = hijoSquare.gameObject;
            punt_atac.SetActive(false);
        }

        if (hijoColisionador != null)
        {
            Collider2D col = hijoColisionador.GetComponent<Collider2D>();
            if (col != null) col.isTrigger = true;
            hijoColisionador.gameObject.tag = "PlayerHitbox";
        }
    }

    void Update()
    {
        if (currentstate == playerstate.dead) return;

        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (currentstate == playerstate.attacking)
        {
            currentAttackTimer -= Time.deltaTime;
            if (currentAttackTimer <= 0) EndAttack();
            return;
        }

        if (keyboard.mKey.wasPressedThisFrame && able_to_attack)
        {
           if(StartAttack())
            {
                anim.SetBool("isPunch", true);
                anim.SetBool("isIdle", false);
                anim.SetBool("isRunning", false);
            
            }
            else
            {
                anim.SetBool("isPunch", false);
            }
            return;
        } 
        else 
        {
            anim.SetBool("isPunch", false);
        }

        bool mov =  HandleMovement();
        if (!mov){
            anim.SetBool("isIdle", true);
            anim.SetBool("isRunning", false);
        }
        else
        {
            anim.SetBool("isIdle", false);
            anim.SetBool("isRunning", true);
        }

        // Estado de animación/estado
        if (rb.linearVelocity.magnitude > 0.1f)
            currentstate = playerstate.running;
        else
            currentstate = playerstate.idle;
        
    }

    
    bool HandleMovement()
    {
        var keyboard = Keyboard.current;
        float moveX = (keyboard.aKey.isPressed ? -1 : 0) + (keyboard.dKey.isPressed ? 1 : 0);
        float moveY = (keyboard.sKey.isPressed ? -1 : 0) + (keyboard.wKey.isPressed ? 1 : 0);

        if (moveX < 0 && !lookingLeft) Flip();
        else if (moveX > 0 && lookingLeft) Flip();

        rb.linearVelocity = new Vector2(moveX, moveY).normalized * speed;

        return moveX != 0 || moveY != 0;
    }   
    
    void Flip()
    {
        lookingLeft = !lookingLeft;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    bool StartAttack()
    {
        currentstate = playerstate.attacking;
        able_to_attack = false;
        currentAttackTimer = attackDuration;
        if (punt_atac != null) {
            punt_atac.SetActive(true);
            return true;
        }
        return false;   
        
    }

    public void EndAttack()
    {
        currentstate = playerstate.idle;
        if (punt_atac != null) punt_atac.SetActive(false);
        anim.SetBool("isPunch", false);
        Invoke(nameof(ResetCooldown), attackCooldown);
    }

    void ResetCooldown() => able_to_attack = true;

    public void TakeDamage(float damage)
    {
        health -= damage;
        Debug.Log("Player Health: " + health);
        if (health <= 0)
        {
            currentstate = playerstate.dead;
            rb.linearVelocity = Vector2.zero;
            Debug.Log("Game Over");
            // Aquí podrías usar Destroy(gameObject) o una animación de muerte
            Destroy(gameObject);
        }
    }

    public void Exp_Gained()
    {
        if (level >= xp_lvl.Length) return;
        while (exp >= xp_lvl[level])
        {
            level++;
            if (levelManager != null) levelManager.show_update();
            if (level >= xp_lvl.Length) break;
        }
    }
}