using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int speed = 40;
    public float health = 20f;
    public int detectionRange = 50;
    public int damage = 10;


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
       
        if (distanceToPlayer < detectionRange && distanceToPlayer > 13.5)
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
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Handle collision with player (damage, etc.)
            Debug.Log("Enemy hit player!");
        }
        if (collision.CompareTag("Player_attack"))
        {
            // Get the PlayerControl from the attack collider or its parent and use its damage value
            PlayerControl pc = collision.GetComponentInParent<PlayerControl>();
            if (pc == null)
                pc = FindObjectOfType<PlayerControl>();

            if (pc != null)
                TakeDamage(pc.PlayerDamage);
            else
                TakeDamage(10f); // fallback
        }
    }
}