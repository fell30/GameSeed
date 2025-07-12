using UnityEngine;

public class GoToBaseState : IEnemyState
{
    private Enemy enemy;

    public void Enter(Enemy enemy)
    {
        this.enemy = enemy;
        Debug.Log("GoToBaseState: Moving to base...");

        Vector3 offset = Random.insideUnitSphere * enemy.offsetRange;
        offset.y = 0;

        if (enemy.baseTarget != null)
        {
            enemy.GetAgent().SetDestination(enemy.baseTarget.position + offset);
        }
    }

    public void Update()
    {
        if (enemy.IsPlayerInRange())
        {
            enemy.ChangeState(new ChaseState());
        }
        else if (enemy.IsAtBase())
        {
            enemy.ChangeState(new AttackBaseState());
        }
    }

    public void Exit()
    {
        Debug.Log("Exit GoToBase");
    }
}
