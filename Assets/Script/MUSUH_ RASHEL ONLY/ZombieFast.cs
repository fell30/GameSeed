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
    public LayerMask obstacleLayerMask = -1;

    [Header("References")]
    public Animator animator;
    public NavMeshAgent agent;
    public Transform targetTower;
    public Transform player;
    public Transform targetBase;
    private bool isTowerDestroyed = false;
    public AudioSource audioSource;
    public AudioClip deathClip;
    public AudioClip hitClip;
    public AudioClip KillClip;

    [Header("Health Bar")]
    public EnemyHealthBar healthBar;

    [Header("Effects")]
    public GameObject explosionEffect;
    public Transform explosionPoint;

    private float currentHealth;
    private bool isDead = false;
    private float attackTimer;
    private bool isInitialized = false;

    private enum TargetMode { Player, Tower, Base }
    private TargetMode currentTarget = TargetMode.Tower;

    void Start()
    {
        currentHealth = maxHealth;
        if (targetBase == null)
        {
            GameObject baseObj = GameObject.FindWithTag("Base");
            if (baseObj != null) targetBase = baseObj.transform;
        }
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
        if (targetTower == null)
        {
            isTowerDestroyed = true;
            currentTarget = TargetMode.Base;
        }

        if (currentTarget == TargetMode.Player && player != null && HasLineOfSight(player))
        {
            float distToPlayer = Vector3.Distance(transform.position, player.position);
            Debug.DrawRay(transform.position, (player.position - transform.position).normalized * visionRange, Color.red);
            if (distToPlayer <= agent.stoppingDistance + 1f)
                AttackPlayer();

            else
                MoveTo(player.position);
        }
        // Tambah di Update() setelah kondisi Player
        else if (currentTarget == TargetMode.Base && targetBase != null)
        {
            float distToBase = Vector3.Distance(transform.position, targetBase.position);
            if (distToBase <= agent.stoppingDistance + 0.5f)
                AttackBase();
            else
                MoveTo(targetBase.position);
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

    private bool HasLineOfSight(Transform target)
    {
        Vector3 directionToTarget = (target.position - transform.position).normalized;
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, directionToTarget, out hit, distanceToTarget, obstacleLayerMask))
        {
            // Jika ray mengenai obstacle sebelum sampai ke target
            return false;
        }

        // Tidak ada obstacle yang menghalangi
        return true;
    }
    void UpdateTarget()
    {
        if (player == null) return;
        if (isTowerDestroyed)
        {
            currentTarget = TargetMode.Base;
            return;
        }

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
                if (towerHealth.currentHealth <= 0f)
                {
                    isTowerDestroyed = true;
                    currentTarget = TargetMode.Base;

                }
            }
        }
    }
    void AttackBase()
    {
        agent.isStopped = true;
        animator.SetBool("isRunning", false);
        Debug.Log("Attacking base");
    }

    void AttackPlayer()
    {
        agent.isStopped = true;
        animator.SetBool("isRunning", false);
        animator.SetBool("isAttacking", true);



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


    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
            audioSource.PlayOneShot(clip);
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        animator.SetTrigger("isHit");
        StartCoroutine(TemporaryStop(0.5f));
        PlaySound(hitClip);
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
            healthBar.SetHealth(currentHealth, maxHealth);

        if (currentHealth <= 0f)
        {
            PlaySound(deathClip);
            Die();
        }
    }
    private IEnumerator TemporaryStop(float duration)
    {
        if (agent.enabled)
        {
            agent.isStopped = true;
            yield return new WaitForSeconds(duration);
            if (!isDead)
                agent.isStopped = false;
        }
    }


    void Die()
    {
        isDead = true;
        agent.isStopped = true;
        agent.enabled = false;

        animator.SetTrigger("isDead");
        StartCoroutine(deathSequence());
        PlaySound(deathClip);

    }

    private IEnumerator deathSequence()
    {
        yield return new WaitForSeconds(3f);

        if (explosionEffect != null)
        {
            Destroy(gameObject);
            GameObject explosion = Instantiate(explosionEffect, explosionPoint.position, Quaternion.identity);
            AudioSource.PlayClipAtPoint(deathClip, explosionPoint.position);
            Destroy(explosion, 1f);
        }
    }
}
