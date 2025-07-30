using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class ZombieEnemy : MonoBehaviour
{
    [Header("Zombie Stats")]
    public float maxHealth = 100f;
    public float currentHealth;
    public float moveSpeed = 2f;
    public float attackDamage = 25f;
    public float attackRange = 1.5f;
    public float attackCooldown = 2f;

    [Header("References")]
    public Transform target; // Tower yang akan diserang
    public Animator animator;
    public NavMeshAgent navAgent;
    public AudioSource audioSource;

    [Header("Audio Clips")]
    public AudioClip[] zombieGroanSounds;
    public AudioClip[] zombieAttackSounds;
    public AudioClip zombieDeathSound;
    public AudioClip zombieDamageSound;

    [Header("Effects")]
    public GameObject bloodEffectPrefab;
    public GameObject deathEffectPrefab;
    public Transform bloodSpawnPoint;

    // Private variables
    private bool isAlive = true;
    private bool isAttacking = false;
    private bool canAttack = true;
    private float lastAttackTime;
    private TowerHealth towerHealth;

    // Animation state hashes (lebih efisien dari string)
    private int walkHash = Animator.StringToHash("isWalking");
    private int attackHash = Animator.StringToHash("Attack");
    private int damageHash = Animator.StringToHash("TakeDamage");
    private int deathHash = Animator.StringToHash("Death");
    private int speedHash = Animator.StringToHash("Speed");

    void Start()
    {
        InitializeZombie();
    }

    void InitializeZombie()
    {
        currentHealth = maxHealth;

        // Setup NavMesh Agent
        if (navAgent == null)
            navAgent = GetComponent<NavMeshAgent>();
        navAgent.speed = moveSpeed;
        navAgent.stoppingDistance = attackRange;

        // Setup Animator
        if (animator == null)
            animator = GetComponent<Animator>();

        // Setup Audio
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        // Find tower target jika belum di-assign
        if (target == null)
            FindNearestTower();

        // Get tower health component
        if (target != null)
            towerHealth = target.GetComponent<TowerHealth>();

        // Start walking animation
        animator.SetBool(walkHash, true);
        animator.SetFloat(speedHash, 1f);

        // Play random groan sound
        PlayRandomGroanSound();
    }

    void Update()
    {
        if (!isAlive) return;

        HandleMovement();
        HandleAttack();
    }

    void HandleMovement()
    {
        if (target == null || isAttacking) return;

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (distanceToTarget > attackRange)
        {
            // Bergerak menuju tower
            navAgent.SetDestination(target.position);
            animator.SetBool(walkHash, true);
            animator.SetFloat(speedHash, navAgent.velocity.magnitude / navAgent.speed);
        }
        else
        {
            // Berhenti dan siap menyerang
            navAgent.SetDestination(transform.position);
            animator.SetBool(walkHash, false);
            animator.SetFloat(speedHash, 0f);
        }
    }

    void HandleAttack()
    {
        if (target == null || !isAlive) return;

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (distanceToTarget <= attackRange && canAttack && !isAttacking)
        {
            StartCoroutine(AttackSequence());
        }
    }

    IEnumerator AttackSequence()
    {
        isAttacking = true;
        canAttack = false;

        // Face the tower
        Vector3 direction = (target.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(direction);

        // Play attack animation
        animator.SetTrigger(attackHash);

        // Play attack sound
        PlayRandomAttackSound();

        // Wait for animation to reach damage point (sekitar 0.5 detik)
        yield return new WaitForSeconds(0.5f);

        // Deal damage to tower
        if (towerHealth != null && Vector3.Distance(transform.position, target.position) <= attackRange)
        {
            towerHealth.TakeDamage(attackDamage);
        }

        // Wait for attack animation to finish
        yield return new WaitForSeconds(1f);

        isAttacking = false;

        // Wait for cooldown
        yield return new WaitForSeconds(attackCooldown);

        canAttack = true;
    }

    public void TakeDamage(float damage)
    {
        if (!isAlive) return;

        currentHealth -= damage;

        // Play damage animation
        animator.SetTrigger(damageHash);

        // Play damage sound
        PlayDamageSound();

        // Spawn blood effect
        SpawnBloodEffect();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        if (!isAlive) return;

        isAlive = false;

        // Stop navigation
        navAgent.enabled = false;

        // Play death animation
        animator.SetTrigger(deathHash);

        // Play death sound
        PlayDeathSound();

        // Spawn death effect
        SpawnDeathEffect();

        // Start death sequence
        StartCoroutine(DeathSequence());
    }

    IEnumerator DeathSequence()
    {
        // Wait for death animation
        yield return new WaitForSeconds(3f);

        // Fade out atau destroy
        StartCoroutine(FadeAndDestroy());
    }

    IEnumerator FadeAndDestroy()
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        float fadeDuration = 2f;
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);

            foreach (Renderer renderer in renderers)
            {
                if (renderer.material.HasProperty("_Color"))
                {
                    Color color = renderer.material.color;
                    color.a = alpha;
                    renderer.material.color = color;
                }
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Destroy zombie
        Destroy(gameObject);
    }

    void SpawnBloodEffect()
    {
        if (bloodEffectPrefab != null && bloodSpawnPoint != null)
        {
            GameObject blood = Instantiate(bloodEffectPrefab, bloodSpawnPoint.position, bloodSpawnPoint.rotation);
            Destroy(blood, 2f);
        }
    }

    void SpawnDeathEffect()
    {
        if (deathEffectPrefab != null)
        {
            GameObject deathEffect = Instantiate(deathEffectPrefab, transform.position, transform.rotation);
            Destroy(deathEffect, 5f);
        }
    }

    void PlayRandomGroanSound()
    {
        if (zombieGroanSounds.Length > 0 && audioSource != null)
        {
            AudioClip randomGroan = zombieGroanSounds[Random.Range(0, zombieGroanSounds.Length)];
            audioSource.PlayOneShot(randomGroan, 0.3f);
        }
    }

    void PlayRandomAttackSound()
    {
        if (zombieAttackSounds.Length > 0 && audioSource != null)
        {
            AudioClip randomAttack = zombieAttackSounds[Random.Range(0, zombieAttackSounds.Length)];
            audioSource.PlayOneShot(randomAttack, 0.7f);
        }
    }

    void PlayDamageSound()
    {
        if (zombieDamageSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(zombieDamageSound, 0.5f);
        }
    }

    void PlayDeathSound()
    {
        if (zombieDeathSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(zombieDeathSound, 0.8f);
        }
    }

    void FindNearestTower()
    {
        GameObject[] towers = GameObject.FindGameObjectsWithTag("Tower");
        float nearestDistance = Mathf.Infinity;

        foreach (GameObject tower in towers)
        {
            float distance = Vector3.Distance(transform.position, tower.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                target = tower.transform;
            }
        }
    }

    // Method untuk debugging
    void OnDrawGizmosSelected()
    {
        // Attack range
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // Path ke target
        if (target != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, target.position);
        }
    }

    // Method untuk dipanggil dari luar (misalnya dari weapon system)
    public bool IsAlive()
    {
        return isAlive;
    }

    public float GetHealthPercentage()
    {
        return currentHealth / maxHealth;
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        if (target != null)
            towerHealth = target.GetComponent<TowerHealth>();
    }
}