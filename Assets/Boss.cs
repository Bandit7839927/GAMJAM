using UnityEngine;

public class Boss : MonoBehaviour
{
    [Header("Stats")]
    public int speed = 20;
    public float health = 20f;
    public int damage = 10;
    public float detectionRange = 50f;
    public float attackRange = 15f; 
    public int xp_drop = 5;

    [Header("Attack Settings")]
    public float damageInterval = 1.0f; 
    public float punchDuration = 0.3f;
    private float nextAttackAllowedTime = 0f;
    private float nextDamageTimeToPlayer = 0f; 
    private float punchEndTime = 0f; 

    [Header("Audio")]
    public AudioSource punch; // Assigna un AudioSource amb el clip de cop
    public AudioSource death; // Assigna un AudioSource amb el clip de mort

    private Transform player;
    private Rigidbody2D rb;
    private float nextTimeEnemyCanBeHit = 0f;
    private Animator anim;
    private bool lookingLeft = true;
    private bool isDead = false; // Control per no morir dues vegades

    public int idMascaraADesbloquejar = 1; 

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
        if (player == null || isDead) return;

        float dist = Vector2.Distance(transform.position, player.position);
        bool isPunching = Time.time < punchEndTime;

        if (isPunching) 
        {
            if (!anim.GetBool("IsPunch")) anim.SetBool("IsPunch", true);
            anim.SetBool("IsWalk", false);
            rb.linearVelocity = Vector2.zero; 
            return; 
        }

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
            
            // Reproduir so de cop
            if (punch
     != null) punch
    .Play();

            Debug.Log("BOSS Ataca!");
        }
        else { anim.SetBool("IsPunch", false); }
    }

    public void TakeDamage(float amount)
    {
        if (Time.time < nextTimeEnemyCanBeHit || isDead) return; 
        health -= amount;
        nextTimeEnemyCanBeHit = Time.time + 0.1f; 

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        anim.enabled = false;

        // So de mort (fem servir PlayClipAtPoint perquè el so no es talli al destruir l'objecte)
        if (death != null)
        {
            death.Play();
        } //Explotar();

        PlayerControl pc = player.GetComponent<PlayerControl>();
        if (pc != null)
        {
            pc.exp += xp_drop;
            pc.Exp_Gained();
            pc.DesbloquejarMascara(idMascaraADesbloquejar);
            Debug.Log("BOSS Derrotat! Mascara " + idMascaraADesbloquejar + " desbloquejada.");
        }

        Destroy(gameObject);
    }

    void HandlePlayerDamage(GameObject obj)
    {
        if (Time.time >= nextDamageTimeToPlayer && !isDead)
        {
            PlayerControl pc = obj.GetComponentInParent<PlayerControl>();
            if (pc != null)
            {
                pc.TakeDamage(damage);
                nextDamageTimeToPlayer = Time.time + damageInterval;
                StartPunchAnimation(); 
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

        if (collision.CompareTag("Projectil"))
        {
            DadesProjectil dades = collision.GetComponent<DadesProjectil>();
            if (dades != null)
            {
                TakeDamage(dades.damage);
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