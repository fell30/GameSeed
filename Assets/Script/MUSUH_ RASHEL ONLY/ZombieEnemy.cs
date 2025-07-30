using UnityEngine;
using UnityEngine.AI;

public class ZombieEnemy : MonoBehaviour
{
    [Header("Zombie Stats")]
    public float maxHealth = 100f;
    public float attackDamage = 10f;
    public float attackInterval = 1.5f;

    [Header("VFX setting")]
    [SerializeField] private GameObject hitEffect;

    [Header("References")]
    public Animator animator;
    public NavMeshAgent agent;
    public Transform targetTower;

    private float currentHealth;
    private bool isDead = false;
    private float attackTimer;

    void Start()
    {
        currentHealth = maxHealth;
        agent.SetDestination(targetTower.position);
    }

    void Update()
    {
        if (isDead) return;

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

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        animator.SetTrigger("isHit");
        if (hitEffect != null)
        {
            Instantiate(hitEffect, transform.position, Quaternion.identity);
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
        animator.SetTrigger("isDead");
        Destroy(gameObject, 3f);
    }
}
