using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float speed = 2f;
    public float health = 10f;
    public float detectionRange = 5000f;

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

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        ChasePlayer();
        // Chase player if in range
        if (distanceToPlayer < detectionRange)
        {
            
        }
        else
        {
            // Patrol
            //rb.linearVelocity = new Vector2(speed * (facingRight ? 1 : -1), rb.linearVelocity.y);
        }
    }

    void ChasePlayer()
    {
        float direction = Mathf.Sign(player.position.x - transform.position.x);
        rb.linearVelocity = new Vector2(speed * direction, rb.linearVelocity.y);

        // Flip sprite if needed
        if ((direction > 0 && !facingRight) || (direction < 0 && facingRight))
        {
            Flip();
        }
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