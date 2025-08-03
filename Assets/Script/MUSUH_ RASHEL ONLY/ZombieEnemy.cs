using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ZombieEnemy : MonoBehaviour
{
    [Header("Zombie Stats")]
    public float maxHealth = 100f;
    public float attackDamage = 10f;
    public float attackInterval = 1.5f;
    public float hitStopDuration = 0.9f;

    [Header("References")]
    public Animator animator;
    public NavMeshAgent agent;
    public Transform targetTower;
    public Transform targetBase; // TARGET BASE BARU

    [Header("Health Bar")]
    public EnemyHealthBar healthBar;

    [Header("Effects")]
    public GameObject explosionEffect;
    public Transform explosionPoint;
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip KillClip;
    public AudioClip hitClip;
    public AudioClip deathClip;

    private float currentHealth;
    private bool isDead = false;
    private float attackTimer;
    private bool isInitialized = false;
    private bool towerDestroyed = false; // FLAG UNTUK CEK APAKAH TOWER SUDAH HANCUR
    private bool isStunned = false; // FLAG UNTUK CEK APAKAH ZOMBIE SEDANG STUN DARI HIT

    private enum TargetMode { Tower, Base } // ENUM UNTUK MODE TARGET
    private TargetMode currentTarget = TargetMode.Tower;

    void Start()
    {
        currentHealth = maxHealth;

        // CARI BASE JIKA BELUM DI-SET
        if (targetBase == null)
        {
            GameObject baseObj = GameObject.FindWithTag("Base");
            if (baseObj != null) targetBase = baseObj.transform;
        }

        if (targetTower != null)
        {
            Initialize();
        }
    }

    void Update()
    {
        if (isDead || !isInitialized || isStunned) return; // TAMBAHKAN CEK isStunned

        if (targetTower == null)
        {
            towerDestroyed = true;
            currentTarget = TargetMode.Base;
        }

        // CEK TARGET BERDASARKAN MODE
        if (towerDestroyed && currentTarget == TargetMode.Base && targetBase != null)
        {
            float distanceToBase = Vector3.Distance(transform.position, targetBase.position);
            if (distanceToBase <= agent.stoppingDistance + 0.5f)
            {
                AttackBase();
            }
            else
            {
                WalkToBase();
            }
        }
        else
        {
            float distance = Vector3.Distance(transform.position, targetTower.position);
            if (distance <= agent.stoppingDistance + 0.5f)
            {
                AttackTower();
            }
            else
            {
                WalkToTower();
            }
        }
    }

    void WalkToTower()
    {
        agent.isStopped = false;
        agent.SetDestination(targetTower.position);
        animator.SetBool("isWalking", true);
        animator.SetBool("isAttacking", false);
    }

    void WalkToBase()
    {
        agent.isStopped = false;
        agent.SetDestination(targetBase.position);
        animator.SetBool("isWalking", true);
        animator.SetBool("isAttacking", false);
    }

    void AttackTower()
    {
        agent.isStopped = true;
        animator.SetBool("isWalking", false);
        animator.SetBool("isAttacking", true);

        TowerHealth towerHealth = targetTower.GetComponent<TowerHealth>();
        attackTimer += Time.deltaTime;
        if (attackTimer >= attackInterval)
        {
            attackTimer = 0f;
            if (towerHealth != null)
            {
                towerHealth.TakeDamage(attackDamage);
                // CEK APAKAH TOWER SUDAH HANCUR
                if (towerHealth.currentHealth <= 0f)
                {
                    towerDestroyed = true;
                    currentTarget = TargetMode.Base;
                }
            }
        }
    }

    void AttackBase()
    {
        agent.isStopped = true;
        animator.SetBool("isWalking", false);
    }

    public void SetTargetTower(Transform tower)
    {
        targetTower = tower;
        isInitialized = true;
    }

    private void Initialize()
    {
        if (isInitialized) return;

        agent.SetDestination(targetTower.position);
        healthBar.SetHealth(currentHealth, maxHealth);
        isInitialized = true;
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        animator.SetTrigger("isHit");
        if (audioSource != null && hitClip != null)
        {
            audioSource.PlayOneShot(hitClip);
        }

        // MULAI HIT STOP SEQUENCE
        StartCoroutine(HitStopSequence());

        if (DamageTextManager.Instance != null)
        {
            Vector3 damagePosition = transform.position + Vector3.up * 2f;

            DamageType damageType = DamageType.Normal;
            if (damage >= attackDamage * 1.8f)
                damageType = DamageType.Headshot;
            else if (damage >= attackDamage * 1.3f)
                damageType = DamageType.Critical;

            DamageTextManager.Instance.ShowDamageText(damagePosition, damage, damageType);
        }

        if (healthBar != null)
        {
            healthBar.SetHealth(currentHealth, maxHealth);
        }

        if (currentHealth <= 0f)
        {
            Die();
            if (audioSource != null && KillClip != null)
            {
                audioSource.PlayOneShot(KillClip);
            }
        }
    }

    // COROUTINE UNTUK MEMBERIKAN JEDA SETELAH KENA HIT
    private IEnumerator HitStopSequence()
    {
        isStunned = true;
        agent.isStopped = true;
        animator.SetBool("isWalking", false);
        animator.SetBool("isAttacking", false);

        yield return new WaitForSeconds(hitStopDuration);

        if (!isDead) // PASTIKAN ZOMBIE MASIH HIDUP
        {
            isStunned = false;
            // Zombie akan otomatis melanjutkan gerakan di Update()
        }
    }

    void Die()
    {
        isDead = true;
        agent.isStopped = true;
        agent.enabled = false; // wajib

        animator.SetTrigger("isDead");
        StartCoroutine(deathSequenece());
    }

    private IEnumerator deathSequenece()
    {
        yield return new WaitForSeconds(3f);
        if (explosionEffect != null)
        {
            Destroy(gameObject);
            GameObject explosion = Instantiate(explosionEffect, explosionPoint.position, Quaternion.identity);
            if (audioSource != null && deathClip != null)
            {
                AudioSource.PlayClipAtPoint(deathClip, explosionPoint.position);
            }
            Destroy(explosion, 1f);
        }
    }
}