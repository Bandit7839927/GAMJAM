using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement; 

public class PlayerControl : MonoBehaviour
{
    public int currMask = 0; 

    [Header("Progresso")]
    public List<int> mascaresDesbloquejades = new List<int> { 0 };

    [Header("Stats de la Run")]
    public float speed = 35f;
    public float health = 100f;
    public float MaxHealth = 100f;
    public float shield = 10f;
    public float playerDamage = 10f;

    [Header("Millores Meta (Persistents per Màscara)")]
    public float playerDamageExtra = 0f;
    public float maxHealthExtra = 0f;

    [Header("Atac")]
    public GameObject punt_atac;
    public float attackCooldown = 0.1f;
    public float attackDuration = 0.2f;

    private float currentAttackTimer;
    private bool able_to_attack = true;
    private bool lookingLeft = false;

    [Header("Inventari")]
    public GameObject itemInventari;
    public UnityEngine.UI.Image slotInventoriUI;
    public ObjecteRecollible objecteAprop;

    enum playerstate { idle, running, dead, attacking, parrying }
    private Animator anim;
    playerstate currentstate = playerstate.idle;

    [Header("Nivells")]
    public int level = 0;
    public int exp = 0;
    public int[] xp_lvl = { 3, 5, 10, 15, 40 };
    public levelUpHandler levelManager;
    public MaskHandler MaskManager;
    private Rigidbody2D rb;

    [Header("Audio Settings")]
    public AudioSource punch; 
    public AudioSource death;

    // --- LOGICA DE PROGRESSIÓ ---

    public void DesbloquejarMascara(int id) {
        if (!mascaresDesbloquejades.Contains(id)) {
            mascaresDesbloquejades.Add(id);
            PlayerPrefs.SetInt("MascaraUnlocked_" + id, 1);
            PlayerPrefs.Save();
            Debug.Log("Nova màscara desbloquejada: " + id);
        }
    }

    public void GuardarMilloresMascara(int idMascara)
    {
        PlayerPrefs.SetFloat("Mascara_" + idMascara + "_DanyExtra", playerDamageExtra);
        PlayerPrefs.SetFloat("Mascara_" + idMascara + "_VidaExtra", maxHealthExtra);
        PlayerPrefs.Save();
    }

    public void ResetStatsBase()
    {
        speed = 35f;
        MaxHealth = 100f;
        playerDamage = 10f;
        shield = 10f;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gameObject.tag = "Player";
        anim = GetComponent<Animator>();
        
        if (punt_atac != null) punt_atac.SetActive(false);

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

        if (levelManager == null) levelManager = FindFirstObjectByType<levelUpHandler>();
        
        if (slotInventoriUI == null)
        {
            GameObject objSlot = GameObject.Find("slot_inventory"); 
            if (objSlot != null) slotInventoriUI = objSlot.GetComponent<UnityEngine.UI.Image>();
        }

        MaskManager = FindFirstObjectByType<MaskHandler>();
        // Netejem la llista i ens assegurem que la 0 (Default) hi sigui
        if (!mascaresDesbloquejades.Contains(0)) mascaresDesbloquejades.Add(0);

        // Suposem que tens fins a 10 màscares. Fem un bucle per comprovar quines hem guardat.
        for (int i = 1; i <= 10; i++) 
        {
            // Si el PlayerPrefs diu que la màscara 'i' està desbloquejada (valor 1)
            if (PlayerPrefs.GetInt("MascaraUnlocked_" + i, 0) == 1) 
            {
                if (!mascaresDesbloquejades.Contains(i)) 
                {
                    mascaresDesbloquejades.Add(i);
                    Debug.Log("MÀSCARA RECUPERADA DEL DISCC: " + i);
                }
            }
        }

        // Un cop tenim la llista plena, actualitzem el menú de selecció si existeix
        if (MaskManager != null) MaskManager.show_update_mask();
    }

    void Update()
    {
        if (anim != null) anim.SetFloat("animMask", currMask);
        if (currentstate == playerstate.dead) return;

        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        // 1. RECOLLIR / LLANÇAR (N) - Ho posem dalt de tot perquè sempre funcioni
        if (keyboard.nKey.wasPressedThisFrame)
        {
            if (itemInventari != null) LlançarItem();
            else RecollirItem();
        }

        // 2. ATAC RÀPID (P)
        if (keyboard.pKey.wasPressedThisFrame && punch != null) punch.Play();

        // 3. LÒGICA D'ESTAT D'ATAC
        if (currentstate == playerstate.attacking)
        {
            currentAttackTimer -= Time.deltaTime;
            
            // Forcem animacions d'atac
            anim.SetBool("isPunch", true); 
            anim.SetBool("isIdle", false);
            anim.SetBool("isRunning", false);

            if (currentAttackTimer <= 0) EndAttack();
            return; // Mentre ataquem, no ens movem
        }

        // 4. INICIAR ATAC (M)
        if (keyboard.mKey.wasPressedThisFrame && able_to_attack)
        {
            if (StartAttack())
            {   
                anim.SetBool("isPunch", true);
                return; 
            }
        } 

        // 5. MOVIMENT (Només si no estem atacant)
        bool mov = HandleMovement();
        anim.SetBool("isIdle", !mov);
        anim.SetBool("isRunning", mov);

        if (rb.linearVelocity.magnitude > 0.1f) currentstate = playerstate.running;
        else currentstate = playerstate.idle;
    }

    private void RecollirItem()
    {
        if (objecteAprop != null && itemInventari == null)
        {
            itemInventari = objecteAprop.prefabDeLobjecte;
            ActualitzarUI(); 
            Destroy(objecteAprop.gameObject);
            objecteAprop = null;
            Debug.Log("Objecte recollit!");
        }
    }

    void ActualitzarUI()
    {
        if (slotInventoriUI == null) return;

        if (itemInventari != null)
        {
            DadesProjectil dades = itemInventari.GetComponent<DadesProjectil>();
            Sprite icona = (dades != null) ? dades.iconaUI : itemInventari.GetComponent<SpriteRenderer>()?.sprite;

            if (icona != null) {
                slotInventoriUI.sprite = icona;
                slotInventoriUI.color = Color.white;
                slotInventoriUI.gameObject.SetActive(true);
            }
        }
        else
        {
            Sprite fonsBuit = Resources.Load<Sprite>("Boto");
            if (fonsBuit != null) {
                slotInventoriUI.sprite = fonsBuit;
                slotInventoriUI.color = Color.white;
            } else {
                slotInventoriUI.color = new Color(1, 1, 1, 0); 
            }
        }
    }

    void LlançarItem()
    {
        if (itemInventari == null) return;
        float direccioX = lookingLeft ? -1f : 1f;
        Vector3 puntSortida = transform.position + new Vector3(direccioX * 1.5f, 0, 0);
        GameObject projectil = Instantiate(itemInventari, puntSortida, Quaternion.identity);
        Rigidbody2D rbProj = projectil.GetComponent<Rigidbody2D>();
        DadesProjectil dades = itemInventari.GetComponent<DadesProjectil>();
        float f = (dades != null) ? dades.velocitatLlançament : 10f;
        if (rbProj != null) rbProj.linearVelocity = new Vector2(direccioX * f, 0);
        itemInventari = null;
        ActualitzarUI();
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
        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
    }

    bool StartAttack()
    {
        currentstate = playerstate.attacking;
        able_to_attack = false;
        currentAttackTimer = attackDuration;
        if (punch != null) punch.Play();
        if (punt_atac != null) { punt_atac.SetActive(true); return true; }
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
        if (health <= 0) Die();
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

    void Die()
    {
        currentstate = playerstate.dead;
        rb.linearVelocity = Vector2.zero;

        // 1. So de mort
        if (death != null)
        {
            death.transform.parent = null;
            death.Play();
            Destroy(death.gameObject, death.clip.length);
        }

        // 2. Guardem el progrés acumulat en la màscara actual abans de sortir
        GuardarMilloresMascara(currMask);

        // 3. Tornem al menú principal
        // IMPORTANT: Assegura't que l'escena estigui afegida a "Build Settings"
        SceneManager.LoadScene("Mainmenu");
    }

    public void EquipMask(MaskData dades)
    {
        ResetStatsBase();
        currMask = dades.idMask;
        playerDamageExtra = PlayerPrefs.GetFloat("Mascara_" + dades.idMask + "_DanyExtra", 0);
        maxHealthExtra = PlayerPrefs.GetFloat("Mascara_" + dades.idMask + "_VidaExtra", 0);
        playerDamage = dades.baseDamage + playerDamageExtra;
        MaxHealth = dades.baseHealth + maxHealthExtra;
        health = MaxHealth; 
        speed = dades.baseSpeed;
        shield = dades.baseShield;

        if (anim != null)
        {
            anim.SetFloat("animMask", currMask);
            anim.Play(anim.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, 0);
        }
    }
}