using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeaponManager : MonoBehaviour
{
  [Header("References")]
  [SerializeField] private EnemyStats enemyStats;

  [SerializeField] private WeaponData _weaponData;
  public WeaponData weaponData
  {
    get => _weaponData; set => _weaponData = value;
  }

  [SerializeField] private Transform firePoint;
  public int currentAmmo;
  private float fireTimer;

  private bool isReloading;
  private float reloadTimer;

  [SerializeField] private GameObject bulletPrefab;
  [SerializeField] private GameObject bulletShellPrefab;

  private void Awake()
  {
    enemyStats = GetComponent<EnemyStats>();
  }

  private void Start()
  {
    currentAmmo = _weaponData.clipSize;
    enemyStats.rotationMultiplier = _weaponData.rotationMultiplier;
    enemyStats.dashPowerMultiplier = _weaponData.dashPowerMultiplier;
  }

  private void Update()
  {
    HandleFireRate();
    HandleReload();
  }

  public void Fire()
  {
    if (currentAmmo > 0 && fireTimer <= 0)
    {
      --currentAmmo;

      Quaternion fireAngle = GetFireAngle();

      // create bullet object
      GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, fireAngle);

      Bullet bulletScript = bulletObj.GetComponent<Bullet>();
      if (bulletScript != null)
      {
        bulletScript.Init(_weaponData.damage, _weaponData.bulletSpeed);
      }

      // create bullet shell object
      GameObject bulletShellObj = Instantiate(bulletShellPrefab, firePoint.position, fireAngle);

      fireTimer = _weaponData.fireRate;

      if (currentAmmo <= 0)
      {
        StartReload();
      }
    }
  }

  private Quaternion GetFireAngle()
  {
    float randomAngle = Random.Range(-_weaponData.fireAngle, _weaponData.fireAngle);

    Quaternion rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + randomAngle);

    return rotation;
  }

  private void HandleFireRate()
  {
    if (fireTimer > 0)
    {
      fireTimer -= Time.deltaTime;
    }
  }

  private void StartReload()
  {
    isReloading = true;

    reloadTimer = _weaponData.reloadTime;
  }

  private void HandleReload()
  {
    if (isReloading)
    {
      if (reloadTimer > 0)
      {
        reloadTimer -= Time.deltaTime;
      }
      else
      {
        isReloading = false;

        currentAmmo = _weaponData.clipSize;
      }
    }
  }
}
