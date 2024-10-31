using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PursueState : State
{
  [Header("FSM Vars")]
  [SerializeField] private CombatState combatState;
  [SerializeField] private DeathState deathState;

  public override void OnFSMStateEnter(EnemyManager enemyManager, EnemyStats enemyStats)
  {
    enemyManager.EnableNavAgent();
  }

  public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats)
  {
    if(enemyStats.isDead)
    {
      return deathState;
    }

    enemyManager.EnableNavAgent();

    RotateToTarget(enemyManager, enemyStats);

    if(CheckDistance(enemyManager, enemyStats))
    {
      return combatState;
    }

    return this;
  }

  private void RotateToTarget(EnemyManager enemyManager, EnemyStats enemyStats)
  {
    // compute direction to look at
    Vector2 playerDir = new Vector2(enemyStats.currentTarget.position.x - enemyManager.transform.position.x, enemyStats.currentTarget.position.y - enemyManager.transform.position.y);

    // compute rotation
    float angle = Mathf.Atan2(playerDir.y, playerDir.x) * Mathf.Rad2Deg;
    Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle - 90f));

    // apply rotation
    enemyManager.transform.rotation = Quaternion.Slerp(enemyManager.transform.rotation, targetRotation, enemyStats.rotationSpeed * enemyStats.rotationMultiplier * Time.deltaTime);
  }

  private bool CheckDistance(EnemyManager enemyManager, EnemyStats enemyStats)
  {
    if (Vector2.Distance(enemyManager.transform.position, enemyStats.currentTarget.position) <= enemyManager.weaponManager.weaponData.range)
    {
      return true;
    }

    return false;
  }

  public override void OnFSMStateExit(EnemyManager enemyManager, EnemyStats enemyStats)
  {
    enemyManager.DisableNavAgent();
  }
}
