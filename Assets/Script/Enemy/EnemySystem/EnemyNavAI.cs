using UnityEngine;
using UnityEngine.AI;

public class EnemyNavAI : MonoBehaviour
{
    [Header("Targeting")]
    [SerializeField] private float targetOffsetRadius = 2f;
    [SerializeField] private float reachThreshold = 1.5f; // Jarak dekat ke target untuk deteksi manual

    private Transform target;
    private NavMeshAgent agent;
    private bool hasDestination = false;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        TrySetDestination();
    }

    private void Update()
    {
        if (!hasDestination && target != null)
        {
            TrySetDestination();
        }

        // Cek apakah sudah dekat sekali dengan target (fallback kalau OnTrigger gagal)
        if (target != null && Vector3.Distance(transform.position, target.position) <= reachThreshold)
        {
            ReachPlayerBase();
        }
    }

    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        hasDestination = false;
    }

    private void TrySetDestination()
    {
        if (agent == null || target == null) return;

        Vector3 offset = Random.insideUnitSphere * targetOffsetRadius;
        offset.y = 0;

        Vector3 destination = target.position + offset;
        agent.SetDestination(destination);

        hasDestination = true;
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

        // Optional: Kalau ada sistem health
        // var hp = target.GetComponent<PlayerBaseHealth>();
        // if (hp != null) hp.TakeDamage(1);

        //Destroy(gameObject);
    }
}
