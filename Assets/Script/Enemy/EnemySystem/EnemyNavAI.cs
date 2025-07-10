using UnityEngine;
using UnityEngine.AI;

public class EnemyNavAI : MonoBehaviour
{
    [Header("Targeting")]
    [SerializeField] private float detectPlayerRange = 5f;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float targetOffsetRadius = 2f;
    [SerializeField] private float reachThreshold = 1.5f;

    private Transform player;
    private Transform baseTarget;
    private Transform currentTarget;

    private NavMeshAgent agent;
    private bool chasingPlayer = false;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        // Cari Player secara otomatis saat runtime
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogWarning("Tidak ditemukan GameObject dengan tag 'Player'");
        }

        if (baseTarget != null)
        {
            SetTarget(baseTarget);
        }
    }

    private void Update()
    {
        if (player != null && Vector3.Distance(transform.position, player.position) < detectPlayerRange)
        {
            chasingPlayer = true;
            currentTarget = player;
            agent.SetDestination(player.position);
        }
        else
        {
            if (chasingPlayer)
            {
                chasingPlayer = false;
                currentTarget = baseTarget;
                TrySetDestination();
            }
        }

        if (chasingPlayer && player != null && Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            AttackPlayer();
        }

        if (!chasingPlayer && baseTarget != null && Vector3.Distance(transform.position, baseTarget.position) <= reachThreshold)
        {
            ReachPlayerBase();
        }
    }

    public void SetTarget(Transform newBaseTarget)
    {
        baseTarget = newBaseTarget;
        currentTarget = newBaseTarget;
        TrySetDestination();
    }

    private void TrySetDestination()
    {
        if (agent == null || currentTarget == null) return;

        Vector3 offset = Random.insideUnitSphere * targetOffsetRadius;
        offset.y = 0;

        Vector3 destination = currentTarget.position + offset;
        agent.SetDestination(destination);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerBase"))
        {
            ReachPlayerBase();
        }
    }

    private void ReachPlayerBase()
    {
        Debug.Log("Enemy reached PlayerBase!");
    }

    private void AttackPlayer()
    {
        Debug.Log("Enemy attacked the Player!");
    }
}
