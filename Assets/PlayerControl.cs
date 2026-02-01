using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine.UI;

public class PlayerControl : MonoBehaviour
{

    public int currMask= 0;

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

    public GameObject itemInventari;
    public UnityEngine.UI.Image slotInventoriUI;

    public ObjecteRecollible objecteAprop;

    enum playerstate { idle, running, dead, attacking, parrying }
    private Animator anim;

    playerstate currentstate = playerstate.idle;
    

    [Header("Nivells")]
    public int level = 0;
    public int exp = 0;
    public int[] xp_lvl = { 3, 10, 20, 30, 40 };
    public levelUpHandler levelManager;

    public MaskHandler maskManager;
    
    private Rigidbody2D rb;
    [Header("Audio")]
    [Header("Audio Settings")]
    // Necessitem els dos components AudioSource que es veuen a la teva imatge
    public AudioSource punch; 
    public AudioSource death;
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

        if (levelManager == null)
        {
            levelManager = FindObjectOfType<levelUpHandler>();
        }

        maskManager.show_update_mask();
    }

    
    private void RecollirItem()
   {
       // 1. Hi ha algun objecte als nostres peus?
       if (objecteAprop != null)
       {
           // 2. Tenim l'inventari buit?
           if (itemInventari == null)
           {
               // A. Guardem el prefab
               itemInventari = objecteAprop.prefabDeLobjecte;

               Debug.Log("RECOLLIT! Ara tens a la butxaca: " + itemInventari.name);

               // --- CORRECCIÓ: AFEGEIX AQUESTA LÍNIA ---
               ActualitzarUI(); 
               // ----------------------------------------

               // B. Destruïm l'objecte del terra
               Destroy(objecteAprop.gameObject);

               // C. Netegem la referència
               objecteAprop = null;
           }
           else
           {
               Debug.Log("INVENTARI PLEN! Ja portes alguna cosa.");
           }
       }
       else
       {
           Debug.Log("No hi ha res per recollir aquí.");
       }
   }
    
    void ActualitzarUI()
    {
        // CAS 1: Tenim un objecte a la butxaca?
        if (itemInventari != null && slotInventoriUI != null)
        {
            // Busquem les dades o el sprite
            DadesProjectil dades = itemInventari.GetComponent<DadesProjectil>();

            if (dades != null)
                slotInventoriUI.sprite = dades.iconaUI;
            else
            {
                SpriteRenderer renderer = itemInventari.GetComponent<SpriteRenderer>();
                if (renderer != null) slotInventoriUI.sprite = renderer.sprite;
            }

            // Encenem la imatge
            slotInventoriUI.gameObject.SetActive(true);
            slotInventoriUI.preserveAspect = true;
        }
        // CAS 2: No tenim res (hem llançat l'objecte)
        else if (slotInventoriUI != null)
        {
            // Apaguem la imatge
            slotInventoriUI.gameObject.SetActive(false);
        }
    }

    void LlançarItem()
    {
        // 1. Seguretat: Si no tens res, no facis res
        if (itemInventari == null) return;

        // 2. Calculem la posició de sortida (una mica davant del jugador per no xocar)
        // Si mirem a l'esquerra (-1) o dreta (+1)
        float direccioX = lookingLeft ? -1f : 1f;
        Vector3 puntSortida = transform.position + new Vector3(direccioX * 1.5f, 0, 0); // 1.5 metres davant

        // 3. GENEREM L'OBJECTE (Instantiate)
        GameObject projectil = Instantiate(itemInventari, puntSortida, Quaternion.identity);

        // 4. LI DONEM VELOCITAT
        Rigidbody2D rbProjectil = projectil.GetComponent<Rigidbody2D>();
        
        // Busquem si té configurada una velocitat especial a la fitxa
        DadesProjectil dades = itemInventari.GetComponent<DadesProjectil>();
        float força = (dades != null) ? dades.velocitatLlançament : 10f; // 10 per defecte

        if (rbProjectil != null)
        {
            // Unity 6 fa servir linearVelocity. Si fas servir versió vella, canvia-ho per .velocity
            rbProjectil.linearVelocity = new Vector2(direccioX * força, 0);
        }

        Debug.Log("Llançat: " + itemInventari.name);

        // 5. BUIDEM L'INVENTARI
        itemInventari = null;
        ActualitzarUI(); // Això apagarà la icona de la pantalla
    }
    
    void Update()
    {
        if (currentstate == playerstate.dead) return;

        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            punch.Play();
            Debug.Log("MANUAL PUNCH SOUND");
        }

        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.nKey.wasPressedThisFrame)
        {
            // Prioritat: Si tenim una arma, la llancem primer
            if (itemInventari != null)
            {
                LlançarItem();
            }
            // Si tenim les mans buides, intentem recollir del terra
            else 
            {
                RecollirItem();
            }
        }

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

        if (punch != null)
        {
            punch.Stop(); // Aturem si s'estava reproduint un so anterior
            punch.Play(); // Reproduïm el so actual
            Debug.Log("S'hauria d'escoltar el PUNCH ara");
        }
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
            Die();
            Destroy(gameObject);
        }
    }

    public void Exp_Gained()
    {
        if (level >= xp_lvl.Length) return;
        while (exp >= xp_lvl[level])
        {
            level++;
            
            if (levelManager != null){
                Debug.Log("Level Up! Nuevo nivel: " + level);
                levelManager.show_update();
            } 
            if (level >= xp_lvl.Length) break;
        }
    }
    void Die()
    {
        currentstate = playerstate.dead;
        rb.linearVelocity = Vector2.zero;

        if (death != null)
        {
            death.transform.parent = null; // detach from player
            death.Play();
            Destroy(death.gameObject, death.clip.length);
        }

        Destroy(gameObject);
    }
}