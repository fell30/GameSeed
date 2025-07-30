using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class Enemy : MonoBehaviour
{
    private IEnemyState currentState;
    private NavMeshAgent agent;
    private Animator animator;

    public Transform playerTarget { get; private set; }
    public Transform baseTarget { get; private set; }

    public float detectPlayerRange = 7f;
    public float attackRange = 1.5f;
    public float offsetRange = 1f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        
        if (IsPlayerInRange())
        {
            ChangeState(new ChaseState());
        }
        else
        {
            ChangeState(new GoToBaseState());
        }
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
               Vector3.Distance(transform.position, playerTarget.position) <= attackRange + 0.1f;
    }

    public void SetWalkingAnimation(bool isWalking)
    {
        if (animator != null)
        {
            Debug.Log("SetWalkingAnimation dipanggil, nilai: " + isWalking);
            animator.SetBool("isWalking", isWalking);
        }
        else
        {
            Debug.LogWarning("Animator belum di-assign!");
        }
    }

    public void SetAttackAnimation(bool isAttacking)
    {
        if (animator != null)
        {
            Debug.Log("SetAttackAnimation dipanggil, nilai: " + isAttacking);
            animator.SetBool("isAttacking", isAttacking);
        }
    }
}
