using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IEnemyState
{
    private Enemy enemy;

    public void Enter (Enemy enemy)
    {
        this.enemy = enemy;
        Debug.Log("Idle State Waiting");
    }

    public void Update()
    {
        if (enemy.IsPlayerInRange())
        {
            enemy.ChangeState(new ChaseState());
        }
        else if (enemy.baseTarget != null)
        {
            enemy.ChangeState(new GoToBaseState());
        }
    }

    public void Exit ()
    {
        Debug.Log("Exit");
    }
}
