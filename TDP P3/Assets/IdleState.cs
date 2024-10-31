using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class IdleState : State
{
  [Header("FSM Vars")]
  [SerializeField] private PursueState pursueState;
  [SerializeField] private DeathState deathState;

  [Header("Other References")]
  [SerializeField] private FieldOfView fov;
  [SerializeField] private GameObject playerObj;

  public override void OnFSMStateEnter(EnemyManager enemyManager, EnemyStats enemyStats)
  {
    enemyStats.currentTarget = null;
  }

  public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats)
  {
    if (enemyStats.isDead)
    {
      return deathState;
    }

    if (HandleDetection(enemyStats))
    {
      Debug.Log("Player Detected");

      return pursueState;
    }
    else if (enemyStats.currentTarget != null)
    {
      return pursueState;
    }
    else
    {
      return this;
    }
  }

  private bool HandleDetection(EnemyStats enemyStats)
  {
    if (fov.IsTargetInFieldOfView(playerObj.transform.position))
    {
      enemyStats.currentTarget = playerObj.transform;

      return true;
    }

    return false;
  }

  public override void OnFSMStateExit(EnemyManager enemyManager, EnemyStats enemyStats)
  {

  }
}
