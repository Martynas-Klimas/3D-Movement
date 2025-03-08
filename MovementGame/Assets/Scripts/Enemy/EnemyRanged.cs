using UnityEngine;

public class EnemyRanged : Enemy
{
    [SerializeField] protected GameObject projectile;
    protected override void EnemyAttack()
    {
        if (!hasAttacked && remainingAttackCooldown <= 0f)
        {
            if (Physics.CheckSphere(transform.position, attackDistance, whatIsPlayer))
            {
                Debug.Log("Attacking");

                // Different attack actions for different enemies (e.g., damage player)
                
                remainingAttackCooldown = attackCooldown;
                hasAttacked = true;

                Invoke(nameof(ResetAttack), attackCooldown);  // Reset attack state after cooldown
            }
        }
        else
        {
            // Stay in attack state until cooldown expires
            remainingAttackCooldown -= Time.deltaTime;
        }
    }
}
