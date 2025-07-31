using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class ZombieFast : MonoBehaviour
{
    [Header("Zombie Stats")]
    public float maxHealth = 100f;
    public float attackDamage = 10f;
    public float attackInterval = 1f;

    [Header("Detection")]
    public float visionRange = 12f;

    [Header("References")]
    public Animator animator;
    public NavMeshAgent agent;
    public Transform targetTower;
    public Transform player;
    public AudioSource audioSource;
    public AudioClip attackClip;
    public AudioClip hitClip;
    public AudioClip deathClip;

    [Header("Health Bar")]
    public EnemyHealthBar healthBar;

    [Header("Effects")]
    public GameObject explosionEffect;
    public Transform explosionPoint;

    private float currentHealth;
    private bool isDead = false;
    private float attackTimer;
    private bool isInitialized = false;

    private enum TargetMode { Player, Tower }
    private TargetMode currentTarget = TargetMode.Tower;

    void Start()
    {
        currentHealth = maxHealth;

        if (player == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null) player = p.transform;
        }

        if (targetTower != null)
        {
            Initialize();
        }
    }

    void Update()
    {
        if (isDead || !isInitialized) return;

        UpdateTarget();

        if (currentTarget == TargetMode.Player)
        {
            float distToPlayer = Vector3.Distance(transform.position, player.position);
            if (distToPlayer <= agent.stoppingDistance + 1f)
                AttackPlayer();

            else
                MoveTo(player.position);
        }
        else
        {
            float distToTower = Vector3.Distance(transform.position, targetTower.position);
            if (distToTower <= agent.stoppingDistance + 0.5f)
                AttackTower();
            else
                MoveTo(targetTower.position);
        }
    }

    void UpdateTarget()
    {
        if (player == null) return;

        float dist = Vector3.Distance(transform.position, player.position);
        if (dist <= visionRange)
        {
            currentTarget = TargetMode.Player;
        }
        else
        {
            currentTarget = TargetMode.Tower;
        }
    }

    void MoveTo(Vector3 position)
    {
        agent.isStopped = false;
        agent.SetDestination(position);

        animator.SetBool("isRunning", true);
        animator.SetBool("isAttacking", false);
    }

    void AttackTower()
    {
        agent.isStopped = true;
        animator.SetBool("isRunning", false);
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

    void AttackPlayer()
    {
        agent.isStopped = true;
        animator.SetBool("isRunning", false);
        animator.SetBool("isAttacking", true);
        if (audioSource != null && attackClip != null)
        {
            audioSource.PlayOneShot(attackClip);
        }


        PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
        attackTimer += Time.deltaTime;
        if (attackTimer >= attackInterval)
        {
            attackTimer = 0f;
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
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
        if (healthBar != null)
            healthBar.SetHealth(currentHealth, maxHealth);

        isInitialized = true;
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        animator.SetTrigger("isHit");
        agent.isStopped = true;
        AudioSource.PlayClipAtPoint(hitClip, transform.position);

        if (healthBar != null)
            healthBar.SetHealth(currentHealth, maxHealth);

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        agent.isStopped = true;
        agent.enabled = false;

        animator.SetTrigger("isDead");
        StartCoroutine(deathSequence());
        AudioSource.PlayClipAtPoint(deathClip, transform.position);
    }

    private IEnumerator deathSequence()
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
