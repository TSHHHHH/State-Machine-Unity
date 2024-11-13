using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.Rendering;

public class EnemyStats : CharacterStats
{
    [Header("References")]
    private EnemyWeaponManager weaponManager;
    private EnemyManager enemyManager;

    private PlayerManager playerManager;

    [Header("Awareness Vars")]
    public float viewDistance = 10f;

    [Range(0, 360)] public float viewAngle = 45f;
    public float closeViewDistance = 5f;
    public float closeViewPuffSize;
    public LayerMask targetMask;
    public LayerMask obstacleMask;
    public Transform currentTarget;

    [Header("Flee Vars")]
    [SerializeField] private FleeState fleeState;
    [SerializeField] private float fleeHealthPercentage = 0.25f;

    [Header("Weapon Drop")]
    [SerializeField] private GameObject weaponDropPrefab;

    [Header("UI Vars")]
    [SerializeField] private GameObject enemyHUDPrefab;
    [SerializeField] private Vector3 healthBarOffset;

    private EnemyHUDManager healthBar;

    private void Awake()
    {
        weaponManager = GetComponent<EnemyWeaponManager>();
        enemyManager = GetComponent<EnemyManager>();
    }

    protected override void Start()
    {
        base.Start();

        playerManager = ServiceLocater.GetService<PlayerManager>();

        HealthBarInit();
    }

    protected override void Update()
    {
        base.Update();
    }

    private void HealthBarInit()
    {
        GameObject healthBarObj = Instantiate(enemyHUDPrefab, transform.position + healthBarOffset, Quaternion.identity);

        healthBar = healthBarObj.GetComponent<EnemyHUDManager>();

        healthBar.Init(transform);

        // enemy health bar should not be visible by default
        healthBar.gameObject.SetActive(false);
    }

    public override void HealFixedAmount(int healAmount)
    {
        currentHealth += healAmount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        // update health bar
        UpdateHealthUI();
    }

    public override void HealPercentage(float percentage)
    {
        currentHealth += (int)(maxHealth * percentage);

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        // update health bar
        UpdateHealthUI();
    }

    internal bool isHealth()
    {
        return currentHealth >= (float)maxHealth * fleeHealthPercentage;
    }

    public override void TakeDamage(int damage)
    {
        currentHealth -= damage;

        // if there is no target, set the player as the target
        if (currentTarget == null)
        {
            currentTarget = playerManager.transform;
        }

        // update health bar
        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            OnDeath();
        }
    }

    private void UpdateHealthUI()
    {
        if (healthBar.gameObject.activeSelf == false)
        {
            healthBar.gameObject.SetActive(true);
        }

        healthBar.UpdateHealthDisplay(this);
    }

    protected override void OnDeath()
    {
        currentHealth = 0;

        if(weaponDropPrefab != null)
            DropWeapon();

        // destroy health bar
        Destroy(healthBar.gameObject);

        // destroy enemy game object in the end
        Destroy(gameObject);
    }

    private void DropWeapon()
    {
        GameObject weaponDropObj = Instantiate(weaponDropPrefab, transform.position, Quaternion.identity);

        WeaponPickUp weaponPickUp = weaponDropObj.GetComponent<WeaponPickUp>();
        if (weaponPickUp != null)
        {
            weaponPickUp.Init(weaponManager.weaponData);
        }
    }
}
