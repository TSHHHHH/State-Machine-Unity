using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
  [Header("References")]
  private InputManager inputManager;
  private PlayerStats playerStats;
  private PlayerLocomotion playerLocomotion;

  private void Awake()
  {
    inputManager = GetComponent<InputManager>();
    playerStats = GetComponent<PlayerStats>();
    playerLocomotion = GetComponent<PlayerLocomotion>();
  }

  private void Update()
  {
    inputManager.TickInput(Time.deltaTime);
  }

  private void FixedUpdate()
  {
    float dt = Time.fixedDeltaTime;

    if (!playerStats.isDashing)
    {
      playerLocomotion.HandleMovement();
    }

    playerLocomotion.HandleRotation(dt);
  }
}
