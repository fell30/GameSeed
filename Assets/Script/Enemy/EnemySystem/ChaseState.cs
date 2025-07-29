using UnityEngine;

public class ChaseState : IEnemyState
{
    private Enemy enemy;

    public void Enter(Enemy enemy)
    {
        this.enemy = enemy;
        Debug.Log("ChaseState: Chasing player...");

        //  Aktifkan animasi jalan
        enemy.SetWalkingAnimation(true);
    }

    public void Update()
    {
        if (enemy.playerTarget == null) return;

        enemy.GetAgent().SetDestination(enemy.playerTarget.position);

        if (enemy.IsNearPlayer())
        {
            enemy.ChangeState(new AttackPlayerState());
        }
        else if (!enemy.IsPlayerInRange())
        {
            enemy.ChangeState(new GoToBaseState());
        }
    }

    public void Exit()
    {
        Debug.Log("Exit Chase");

        //  Matikan animasi jalan
        enemy.SetWalkingAnimation(false);
    }
}
