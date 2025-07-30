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

        // Stop movement and set animations
        enemy.GetAgent().isStopped = true;
        enemy.SetWalkingAnimation(false);
        enemy.SetAttackAnimation(true);
    }

    public void Update()
    {
        // Cek jika player menjauh
        if (!enemy.IsNearPlayer())
        {
            enemy.ChangeState(new ChaseState());
            return;
        }

        // Serang berdasarkan cooldown
        timer += Time.deltaTime;

        if (timer >= attackCooldown)
        {
            Debug.Log("Enemy attacks the player!");
            timer = 0f;

            // TODO: Implement damage to player here
            // e.g., player.TakeDamage(damageAmount);
        }
    }

    public void Exit()
    {
        Debug.Log("Exit AttackPlayer");

        // Reset attack animation and resume movement
        enemy.SetAttackAnimation(false);
        enemy.GetAgent().isStopped = false;
    }
}
