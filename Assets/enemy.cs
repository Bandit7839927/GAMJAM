using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    public int speed = 3;
    public float health = 20f;
    public int damage = 10;
    public float detectionRange = 15f;

    [Header("Attack Settings")]
    public float damageInterval = 1.0f; 
    private float nextDamageTimeToPlayer = 0f; // Track when the player can be hit again

    private Transform player;
    private Rigidbody2D rb;
    private float nextTimeEnemyCanBeHit = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
        
        if (rb != null) rb.sleepMode = RigidbodySleepMode2D.NeverSleep;
    }

    void FixedUpdate()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);
        if (dist < detectionRange && dist > 0.5f)
        {
            Vector2 target = Vector2.MoveTowards(rb.position, player.position, speed * Time.fixedDeltaTime);
            rb.MovePosition(target);
        }
    }

    public void TakeDamage(float amount)
    {
        if (Time.time < nextTimeEnemyCanBeHit) return; 
        
        health -= amount;
        nextTimeEnemyCanBeHit = Time.time + 0.2f; 
        
        if (health <= 0) Destroy(gameObject);
    }

    // This handles both the first hit AND the repeat hits reliably
    void HandlePlayerDamage(GameObject obj)
    {
        // Check if enough time has passed since the LAST hit
        if (Time.time >= nextDamageTimeToPlayer)
        {
            PlayerControl pc = obj.GetComponent<PlayerControl>();
            if (pc == null) pc = obj.GetComponentInParent<PlayerControl>();

            if (pc != null)
            {
                pc.TakeDamage(damage);
                // Set the next allowed hit time to (Now + Interval)
                nextDamageTimeToPlayer = Time.time + damageInterval;
                Debug.Log("Hit registered at: " + Time.time);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("PlayerHitbox"))
        {
            HandlePlayerDamage(collision.gameObject);
        }

        if (collision.CompareTag("Player_attack"))
        {
            PlayerControl pc = collision.GetComponentInParent<PlayerControl>();
            if (pc != null) TakeDamage(pc.playerDamage);
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("PlayerHitbox"))
        {
            HandlePlayerDamage(collision.gameObject);
        }
    }
}