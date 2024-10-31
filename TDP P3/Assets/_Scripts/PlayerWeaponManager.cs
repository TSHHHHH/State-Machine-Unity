using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponManager : MonoBehaviour
{
  [Header("References")]
  private PlayerStats playerStats;
  private PlayerWeaponDisplay weaponDisplay;

  [SerializeField] private Transform firePoint;
  [SerializeField] private WeaponData defaultWeapon;
  private WeaponData currentWeapon;
  public int currentAmmo;
  private float fireTimer;

  private bool isReloading;
  private float reloadTimer;

  [SerializeField] private GameObject bulletPrefab;
  [SerializeField] private GameObject bulletShellPrefab;

  private void Awake()
  {
    playerStats = GetComponent<PlayerStats>();
  }

  private void Start()
  {
    weaponDisplay = ServiceLocater.GetService<PlayerWeaponDisplay>();

    currentWeapon = defaultWeapon;
    OnWeaponSwitch();
  }

  public void SwitchWeapon()
  {

  }

  private void OnWeaponSwitch()
  {
    currentAmmo = currentWeapon.clipSize;
    playerStats.rotationMultiplier = currentWeapon.rotationMultiplier;
    playerStats.dashPowerMultiplier = currentWeapon.dashPowerMultiplier;

    // update UI
    weaponDisplay.UpdateAmmoDisplay(currentWeapon, (int)currentAmmo);
  }

  private void Update()
  {
    HandleFireRate();
    HandleReload();
  }

  public void Fire()
  {
    if(currentAmmo > 0 && fireTimer <= 0)
    {
      --currentAmmo;

      Quaternion fireAngle = GetFireAngle();

      // create bullet object
      GameObject bulletObj = Instantiate(bulletPrefab, firePoint.position, fireAngle);

      Bullet bulletScript = bulletObj.GetComponent<Bullet>();
      if(bulletScript != null)
      {
        bulletScript.Init(currentWeapon.damage, currentWeapon.bulletSpeed);
      }

      // create bullet shell object
      GameObject bulletShellObj = Instantiate(bulletShellPrefab, firePoint.position, fireAngle);

      fireTimer = currentWeapon.fireRate;

      // update UI
      weaponDisplay.UpdateAmmoDisplay(currentWeapon, (int)currentAmmo);

      if (currentAmmo <= 0)
      {
        StartReload();
      }
    }
  }

  private Quaternion GetFireAngle()
  {
    float randomAngle = Random.Range(-currentWeapon.fireAngle, currentWeapon.fireAngle);

    Quaternion rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + randomAngle);

    return rotation;
  }

  private void StartReload()
  {
    isReloading = true;

    reloadTimer = currentWeapon.reloadTime;
  }

  private void HandleFireRate()
  {
    if (fireTimer > 0)
    {
      fireTimer -= Time.deltaTime;
    }
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

        currentAmmo = currentWeapon.clipSize;

        // update UI
        weaponDisplay.UpdateAmmoDisplay(currentWeapon, (int)currentAmmo);
      }
    }
  }
}
