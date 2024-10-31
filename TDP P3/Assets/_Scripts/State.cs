using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State : MonoBehaviour
{
  public abstract void OnFSMStateEnter(EnemyManager enemyManager, EnemyStats enemyStats);

  public abstract State Tick(EnemyManager enemyManager, EnemyStats enemyStats);

  public abstract void OnFSMStateExit(EnemyManager enemyManager, EnemyStats enemyStats);
}
