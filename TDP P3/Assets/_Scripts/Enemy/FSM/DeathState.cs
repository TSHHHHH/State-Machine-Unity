using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathState : State
{
  public override void OnFSMStateEnter(EnemyManager enemyManager, EnemyStats enemyStats)
  {

  }

  public override State Tick(EnemyManager enemyManager, EnemyStats enemyStats)
  {
    return this;
  }

  public override void OnFSMStateExit(EnemyManager enemyManager, EnemyStats enemyStats)
  {

  }
}
