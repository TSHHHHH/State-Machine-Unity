using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.Rendering;

public class EnemyStats : CharacterStats
{
    [Header("References")]
    private EnemyWeaponManager weaponManager;

    private PlayerManager playerManager;

    [Header("Awareness Vars")]
    public float viewDistance = 10f;

    [Range(0, 360)] public float viewAngle = 45f;
    public float closeViewDistance = 5f;
    public float closeViewPuffSize;
    public LayerMask targetMask;
    public LayerMask obstacleMask;
    public Transform currentTarget;

    [Header("Weapon Drop")]
    [SerializeField] private GameObject weaponDropPrefab;

    [Header("UI Vars")]
    [SerializeField] private GameObject enemyHUDPrefab;
    [SerializeField] private Vector3 healthBarOffset;

    private EnemyHUDManager healthBar;

    private void Awake()
    {
        weaponManager = GetComponent<EnemyWeaponManager>();
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

        // update health bar position
        if (healthBar != null)
        {
            healthBar.gameObject.transform.position = transform.position + healthBarOffset;
        }
    }

    private void HealthBarInit()
    {
        GameObject healthBarObj = Instantiate(enemyHUDPrefab, transform.position + healthBarOffset, Quaternion.identity);

        healthBar = healthBarObj.GetComponent<EnemyHUDManager>();

        healthBar.Init(transform);

        // enemy health bar should not be visible by default
        healthBar.gameObject.SetActive(false);
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