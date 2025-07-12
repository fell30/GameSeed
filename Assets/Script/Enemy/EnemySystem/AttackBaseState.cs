using UnityEngine;

public class AttackBaseState : IEnemyState
{
    private Enemy enemy;
    private float attackCooldown = 1f;
    private float timer;

    public void Enter(Enemy enemy)
    {
        this.enemy = enemy;
        timer = 0f;
        Debug.Log("AttackBaseState: Attacking base...");
    }

    public void Update()
    {
        timer += Time.deltaTime;

        if (enemy.IsPlayerInRange())
        {
            enemy.ChangeState(new ChaseState());
            return;
        }

        if (timer >= attackCooldown)
        {
            Debug.Log("Enemy attacks the base!");
            timer = 0f;
            // TODO: Apply damage to base here
        }
    }

    public void Exit()
    {
        Debug.Log("Exit AttackBase");
    }
}
