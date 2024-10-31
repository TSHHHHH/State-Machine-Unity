using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderKeywordFilter;
using UnityEngine;
using UnityEngine.Rendering;

public class EnemyStats : CharacterStats
{
  [Header("References")]
  private EnemyWeaponManager weaponManager;

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
  private EnemyHUDManager healthBar;

  private void Awake()
  {
    weaponManager = GetComponent<EnemyWeaponManager>();
  }

  protected override void Start()
  {
    base.Start();

    HealthBarInit();
  }

  private void HealthBarInit()
  {
    GameObject healthBarObj = Instantiate(enemyHUDPrefab, transform.position, Quaternion.identity);

    healthBar = healthBarObj.GetComponent<EnemyHUDManager>();

    healthBar.Init(transform);

    // enemy health bar should not be visible by default
    healthBar.gameObject.SetActive(false);
  }

  protected override void TakeDamage(int damage)
  {
    currentHealth -= damage;

    // update health bar
    UpdateHealthUI();

    if (currentHealth <= 0)
    {
      OnDeath();
    }
  }

  private void UpdateHealthUI()
  {
    if(healthBar.gameObject.activeSelf == false)
    {
      healthBar.gameObject.SetActive(true);
    }

    healthBar.UpdateHealthDisplay(this);
  }

  protected override void OnDeath()
  {
    currentHealth = 0;

    DropWeapon();

    Destroy(gameObject);
  }

  private void DropWeapon()
  {
    GameObject weaponDropObj = Instantiate(weaponDropPrefab, transform.position, Quaternion.identity);

    WeaponPickUp weaponPickUp = weaponDropObj.GetComponent<WeaponPickUp>();
    if(weaponPickUp != null)
    {
      weaponPickUp.Init(weaponManager.weaponData);
    }
  }
}
