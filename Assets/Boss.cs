using UnityEngine;

public class Boss : MonoBehaviour
{
    [Header("Stats")]
    public int speed = 20;
    public float health = 20f;
    public int damage = 10;
    public float detectionRange = 50f;
    public float attackRange = 15f; // Rango para dejar de caminar y golpear

    [Header("Attack Settings")]
    public float damageInterval = 1.0f; 
    public float punchDuration = 0.3f;
    private float nextAttackAllowedTime = 0f;
    private float nextDamageTimeToPlayer = 0f; 
    private float punchEndTime = 0f; // Momento en que debe terminar la animación

    private Transform player;
    private Rigidbody2D rb;
    private float nextTimeEnemyCanBeHit = 0f;
    private Animator anim;
    private bool lookingLeft = true;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
        
        if (rb != null) rb.sleepMode = RigidbodySleepMode2D.NeverSleep;
    }

    void FixedUpdate()
    {
        if (player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);
        bool isPunching = Time.time < punchEndTime;

        if (isPunching) 
        {
            // Forzamos el estado de ataque y salimos
            if (!anim.GetBool("IsPunch")) anim.SetBool("IsPunch", true);
            anim.SetBool("IsWalk", false);
            rb.linearVelocity = Vector2.zero; 
            return; 
        }

        // Si llegamos aquí, NO estamos golpeando (isPunching es false)
        anim.SetBool("IsPunch", false);

        if (dist < detectionRange)
        {
            bool shouldLookLeft = player.position.x > transform.position.x;
            if (shouldLookLeft != lookingLeft) Flip();

            if (dist > attackRange) 
            {
                anim.SetBool("IsWalk", true);
                Vector2 target = Vector2.MoveTowards(rb.position, player.position, speed * Time.fixedDeltaTime);
                rb.MovePosition(target);
            }
            else 
            {
                // Detenemos y atacamos
                rb.linearVelocity = Vector2.zero;
                anim.SetBool("IsWalk", false);
                StartPunchAnimation();
            }
        }
        else 
        {
            anim.SetBool("IsWalk", false);
        }
    }
    void StartPunchAnimation()
    {
        if (Time.time >= nextAttackAllowedTime) 
        {
            anim.SetBool("IsPunch", true);
            
            punchEndTime = Time.time + punchDuration;
            nextAttackAllowedTime = Time.time + damageInterval; 
            
            // Debug para que veas en consola si ataca
            Debug.Log("BOSS Ataca!");
        }
        else {anim.SetBool("IsPunch", false);}
    }
    public void TakeDamage(float amount)
    {
        if (Time.time < nextTimeEnemyCanBeHit) return; 
        health -= amount;
        nextTimeEnemyCanBeHit = Time.time + 0.1f; 
        if (health <= 0) Destroy(gameObject);
    }

    void HandlePlayerDamage(GameObject obj)
    {
        if (Time.time >= nextDamageTimeToPlayer)
        {
            PlayerControl pc = obj.GetComponentInParent<PlayerControl>();
            if (pc != null)
            {
                pc.TakeDamage(damage);
                nextDamageTimeToPlayer = Time.time + damageInterval;
                StartPunchAnimation(); // Activa la animación al tocar al jugador
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

        if (collision.CompareTag("Projectil")) // Assegura't que l'ampolla tingui aquest Tag
        {
            // Busquem les dades del projectil per saber quant mal fa
            DadesProjectil dades = collision.GetComponent<DadesProjectil>();
            
            if (dades != null)
            {
                TakeDamage(dades.damage); // Apliquem el dany definit a l'objecte
                Debug.Log("Enemic colpejat per projectil! Mal rebut: " + dades.damage);
                
                // Opcional: Destruir el projectil en col·lidir amb l'enemic
                Destroy(collision.gameObject); 
            }
        }

    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("PlayerHitbox"))
        {
            HandlePlayerDamage(collision.gameObject);
        }
    }

    void Flip()
    {
        lookingLeft = !lookingLeft;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
