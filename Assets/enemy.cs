using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int speed = 40;
    public float health = 10f;
    public int detectionRange = 50;

    private Transform player;
    private Rigidbody2D rb;
    private bool facingRight = true;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance((Vector2)transform.position, (Vector2)player.position);
        // Chase player if in range
            Debug.Log($"Distance: {distanceToPlayer} | In range: {distanceToPlayer < detectionRange}");
            Debug.Log($"Detection range: {detectionRange}");

        if (distanceToPlayer < detectionRange)
        {
            ChasePlayer();
            
        }
    }

        void ChasePlayer()
    {
        Vector2 targetPosition = Vector2.MoveTowards(
            rb.position,
            player.position,
            speed * Time.fixedDeltaTime
        );

        rb.MovePosition(targetPosition);
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Handle collision with player (damage, etc.)
            Debug.Log("Enemy hit player!");
        }
    }
}