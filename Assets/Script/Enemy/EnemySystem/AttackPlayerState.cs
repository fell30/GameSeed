using UnityEngine;

public class AttackPlayerState : IEnemyState
{
    private Enemy enemy;
    private float attackCooldown = 1f;
    private float timer;

    public void Enter(Enemy enemy)
    {
        this.enemy = enemy;
        timer = 0f;

        Debug.Log("AttackPlayerState: Attacking player...");

        // Aktifkan animasi attack
        enemy.SetWalkingAnimation(false);
        enemy.SetAttackAnimation(true);
    }

    public void Update()
    {
        timer += Time.deltaTime;

        if (!enemy.IsNearPlayer())
        {
            enemy.ChangeState(new ChaseState());
            return;
        }

        if (timer >= attackCooldown)
        {
            Debug.Log("Enemy attacks the player!");
            timer = 0f;

            // TODO: Apply damage to player here
        }
    }

    public void Exit()
    {
        Debug.Log("Exit AttackPlayer");

        // Nonaktifkan animasi attack saat keluar dari state
        enemy.SetAttackAnimation(false);
    }
}
