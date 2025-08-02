using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ZombieEnemy : MonoBehaviour
{
    [Header("Zombie Stats")]
    public float maxHealth = 100f;
    public float attackDamage = 10f;
    public float attackInterval = 1.5f;

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

    private float currentHealth;
    private bool isDead = false;
    private float attackTimer;
    private bool isInitialized = false;
    private bool towerDestroyed = false; // FLAG UNTUK CEK APAKAH TOWER SUDAH HANCUR

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
        if (isDead || !isInitialized) return;
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
        agent.isStopped = true;
        if (DamageTextManager.Instance != null)
        {
            Vector3 damagePosition = transform.position + Vector3.up * 2f;

            // Tentukan damage type berdasarkan damage amount
            DamageType damageType = DamageType.Normal;
            if (damage >= attackDamage * 1.8f) // Damage tinggi = headshot
                damageType = DamageType.Headshot;
            else if (damage >= attackDamage * 1.3f) // Medium = critical
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
            Destroy(explosion, 1f);
        }
    }
}