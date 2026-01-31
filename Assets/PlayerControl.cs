using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerControl : MonoBehaviour
{
    [Header("Stats")]
    public int level = 1;
    public int exp = 0;
    public float speed = 5f;
    public float health = 100f;
    public float shield = 50f;
    public float PlayerDamage = 10f;

    [Header("Atac")]
    public GameObject punt_atac;
    public float attackCooldown = 1f;
    public float attackDuration = 0.2f;

    private float currentAttackTimer;
    private bool able_to_attack = true;
    private bool lookingLeft = false;

    enum playerstate { idle, running, dead, attacking, parrying }
    playerstate currentstate = playerstate.idle;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (punt_atac != null) punt_atac.SetActive(false);
        Transform hijoColisionador = transform.Find("ObjCollider");
    
        if (hijoColisionador != null)
        {
            // Obtenemos su Collider y forzamos el IsTrigger
            Collider2D col = hijoColisionador.GetComponent<Collider2D>();
            if (col != null)
            {
                col.isTrigger = true;
                Debug.Log("Hitbox de recolección configurada como Trigger automáticamente.");
            }
        }
    }

    void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (currentstate == playerstate.attacking)
        {
            currentAttackTimer -= Time.deltaTime;
            if (currentAttackTimer <= 0)
                EndAttack();
            return;
        }

        if (keyboard.mKey.wasPressedThisFrame && able_to_attack)
        {
            StartAttack();
            return;
        }

        HandleMovement();

        if (Keyboard.current.aKey.isPressed ||
            Keyboard.current.dKey.isPressed ||
            Keyboard.current.wKey.isPressed ||
            Keyboard.current.sKey.isPressed)
            currentstate = playerstate.running;
        else
            currentstate = playerstate.idle;
    }

    void HandleMovement()
    {
        var keyboard = Keyboard.current;

        float moveX = (keyboard.aKey.isPressed ? -1 : 0) +
                      (keyboard.dKey.isPressed ? 1 : 0);

        float moveY = (keyboard.sKey.isPressed ? -1 : 0) +
                      (keyboard.wKey.isPressed ? 1 : 0);

        if (moveX < 0 && !lookingLeft) Flip();
        else if (moveX > 0 && lookingLeft) Flip();

        Vector2 movement = new Vector2(moveX, moveY).normalized * speed;
        rb.linearVelocity = movement;
    }

    void Flip()
    {
        lookingLeft = !lookingLeft;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    void StartAttack()
    {
        currentstate = playerstate.attacking;
        able_to_attack = false;
        currentAttackTimer = attackDuration;
        if (punt_atac != null) punt_atac.SetActive(true);
    }

    void EndAttack()
    {
        currentstate = playerstate.idle;
        if (punt_atac != null) punt_atac.SetActive(false);
        CancelInvoke(nameof(ResetCooldown));
        Invoke(nameof(ResetCooldown), attackCooldown);
    }

    void ResetCooldown()
    {
        able_to_attack = true;
    }
}
