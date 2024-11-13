using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Rendering;

public class GrenadorCombatState : CombatState
{
    [Header("Grenador Combat Vars")]
    [SerializeField] private GameObject grenadePrefab;
    [SerializeField] private int throwChance = 50;
    [SerializeField] private float throwCooldown = 5f;
    [SerializeField] private float throwForce = 10f;

    private float throwCooldownTimer = 0f;

    public override void OnFSMStateEnter(EnemyManager enemyManager, EnemyStats enemyStats)
    {
        throwCooldownTimer = throwCooldown;
    }

    public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats)
    {
        if (enemyStats.isDead)
        {
            return deathState;
        }

        if (!enemyStats.isHealth())
        {
            return fleeState;
        }

        RotateToTarget(enemyManager, enemyStats);

        if (enemyManager.HandleDetection())
        {
            enemyManager.Fire();
        }

        // if the player is too far away, return to pursue state
        if (!enemyManager.CheckIfInWeaponFireRange())
        {
            return pursueState;
        }
        else
        {
            // walk around the player
            StrafeMovement(enemyManager, enemyStats);
        }

        HandleGrenadeLogic(enemyManager, enemyStats);

        return this;
    }

    private void HandleGrenadeLogic(EnemyManager enemyManager, EnemyStats enemyStats)
    {
        if(throwCooldownTimer > 0)
        {
            throwCooldownTimer -= Time.deltaTime;
        }
        else
        {
            // roll a dice to see if the enemy throws a grenade
            if (Random.Range(0, 100) < throwChance && enemyManager.HandleDetection())
            {
                // reset the cooldown timer
                throwCooldownTimer = throwCooldown;

                ThrowGrenade(enemyManager, enemyStats);
            }
        }
    }

    private void ThrowGrenade(EnemyManager enemyManager, EnemyStats enemyStats)
    {
        GameObject grenadeObj = Instantiate(grenadePrefab, enemyManager.transform.position, Quaternion.identity);

        Grenade grenadeScript = grenadeObj.GetComponent<Grenade>();
        if(grenadeScript != null)
        {
            // compute the throw direction
            Vector3 throwDir = enemyStats.currentTarget.position - enemyManager.transform.position;
            throwDir.Normalize();

            // multiply the throw direction by the throw force
            throwDir *= throwForce;

            grenadeScript.Init(throwDir);
        }
    }

    protected override void StrafeMovement(EnemyManager enemyManager, EnemyStats enemyStats)
    {
        
    }
}
