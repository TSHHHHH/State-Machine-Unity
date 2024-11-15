using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("References")]
    private InputManager inputManager;
    private PlayerStats playerStats;
    private PlayerLocomotion playerLocomotion;
    private PlayerWeaponManager playerWeaponManager;

    private void Awake()
    {
        ServiceLocater.RegisterService<PlayerManager>(this);

        inputManager = GetComponent<InputManager>();
        playerStats = GetComponent<PlayerStats>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        playerWeaponManager = GetComponent<PlayerWeaponManager>();
    }

    private void Update()
    {
        // if player is dead, do not accept input
        if (playerStats.isDead)
        {
            return;
        }

        inputManager.TickInput(Time.deltaTime);

        playerWeaponManager.UpdateSpreadFeedback();
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

    private void OnDisable()
    {
        ServiceLocater.UnregisterService<PlayerManager>();
    }
}