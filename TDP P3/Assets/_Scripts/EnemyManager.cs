using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyManager : MonoBehaviour
{
  [Header("References")]
  private NavMeshAgent navMeshAgent;
  private EnemyStats enemyStats;
  public EnemyWeaponManager weaponManager;

  [Header("FSM Vars")]
  [SerializeField] private State startState;
  private State currentState;

  private void Awake()
  {
    navMeshAgent = GetComponent<NavMeshAgent>();
    enemyStats = GetComponent<EnemyStats>();
    weaponManager = GetComponent<EnemyWeaponManager>();
  }

  private void Start()
  {
    currentState = startState;
  }

  private void Update()
  {
    HandleFSM();
  }

  private void HandleFSM()
  {
    if (currentState != null)
    {
      State nextState = currentState.Tick(this, enemyStats);

      if (nextState != null)
      {
        SwitchToNextState(nextState);
      }
    }
  }

  private void SwitchToNextState(State nextState)
  {
    if (currentState != nextState)
    {
      currentState.OnFSMStateExit(this, enemyStats);
      currentState = nextState;
      currentState.OnFSMStateEnter(this, enemyStats);
    }
    else
    {
      currentState = nextState;
    }
  }

  public void EnableNavAgent()
  {
    if(navMeshAgent.enabled == false)
      navMeshAgent.enabled = true;

    navMeshAgent.speed = enemyStats.moveSpeed;

    navMeshAgent.destination = enemyStats.currentTarget.position;
  }

  public void DisableNavAgent()
  {
    if(navMeshAgent.enabled == true)
      navMeshAgent.enabled = false;

    navMeshAgent.velocity = Vector3.zero;
  }

  public void Fire()
  {
    weaponManager.Fire();
  }
}
