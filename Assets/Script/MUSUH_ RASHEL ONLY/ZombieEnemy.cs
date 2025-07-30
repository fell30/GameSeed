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
    [Header("Health Bar")]
    public EnemyHealthBar healthBar;
    [Header("Effects")]
    public GameObject explosionEffect;
    public Transform explosionPoint;


    private float currentHealth;
    private bool isDead = false;
    private float attackTimer;
    private bool isInitialized = false;

    void Start()
    {
        if (targetTower != null)
        {
            Initialize();
        }
        currentHealth = maxHealth;
        // agent.SetDestination(targetTower.position);
        // healthBar.SetHealth(currentHealth, maxHealth);


    }

    void Update()
    {
        if (isDead || !isInitialized) return;

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

    void WalkToTower()
    {
        agent.isStopped = false;
        agent.SetDestination(targetTower.position);
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
            }

        }
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
