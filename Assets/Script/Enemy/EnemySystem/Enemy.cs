using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : MonoBehaviour
{
    private IEnemyState currentState;
    private NavMeshAgent agent;

    public Transform playerTarget { get; private set; }
    public Transform baseTarget { get; private set; }

    public float detectPlayerRange = 7f;
    public float attackRange = 1.5f;
    public float offsetRange = 1f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        ChangeState(new IdleState());
    }

    private void Update()
    {
        currentState?.Update();
    }

    public void SetTargets(Transform player, Transform baseTarget)
    {
        this.playerTarget = player;
        this.baseTarget = baseTarget;
    }

    public NavMeshAgent GetAgent() => agent;

    public void ChangeState(IEnemyState newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState.Enter(this);
    }

    public bool IsPlayerInRange()
    {
        return playerTarget != null &&
            Vector3.Distance(transform.position, playerTarget.position) <= detectPlayerRange;
    }

    public bool IsAtBase()
    {
        return baseTarget != null &&
            Vector3.Distance(transform.position, baseTarget.position) <= attackRange;
    }

    public bool IsNearPlayer()
    {
        return playerTarget != null &&
            Vector3.Distance(transform.position, playerTarget.position) <= attackRange;
    }
}
