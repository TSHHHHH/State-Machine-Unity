using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeaponManager : WeaponManager
{
    [Header("References")]
    private PlayerStats playerStats;

    private PlayerWeaponDisplay weaponDisplay;

    [SerializeField] private WeaponData defaultWeapon;

    //[Header("Weapon Drop")]
    //[SerializeField] private GameObject weaponDropPrefab;

    private void Awake()
    {
        playerStats = GetComponent<PlayerStats>();
    }

    protected override void Start()
    {
        base.Start();

        weaponDisplay = ServiceLocater.GetService<PlayerWeaponDisplay>();

        currentWeapon = defaultWeapon;
        OnWeaponSwitch();
    }

    public void SwitchWeapon(WeaponData newWeapon)
    {
        currentWeapon = newWeapon;
        OnWeaponSwitch();
    }

    private void OnWeaponSwitch()
    {
        // reset fire rate
        fireTimer = 0f;

        // reset reloading
        isReloading = false;
        reloadTimer = 0f;

        // set ammo to full
        currentAmmo = currentWeapon.clipSize;

        // update player stats
        playerStats.rotationMultiplier = currentWeapon.rotationMultiplier;
        playerStats.dashPowerMultiplier = currentWeapon.dashPowerMultiplier;

        // update UI
        weaponDisplay.UpdateWeaponDisplay(currentWeapon);
        weaponDisplay.UpdateReloadDisplay(currentAmmo/currentWeapon.clipSize);
        weaponDisplay.UpdateAmmoDisplay(currentWeapon, (int)currentAmmo);
    }

    private void Update()
    {
        HandleFireRate();
        HandleSpreadCooldown();
        HandleReload();
    }

    public override void Fire()
    {
        if (currentAmmo > 0 && fireTimer <= 0)
        {
            isFiring = true;

            --currentAmmo;

            Quaternion fireAngle = GetFireAngle();

            // create bullet object
            GameObject bulletObj = poolManager.SpawnFromPool("Player Bullet", firePoint.position, fireAngle);

            Bullet bulletScript = bulletObj.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.Init(currentWeapon.damage, currentWeapon.bulletSpeed);
            }

            // create bullet shell object
            GameObject bulletShellObj = poolManager.SpawnFromPool("Bullet Shell", firePoint.position, fireAngle);

            BulletShell bulletShellScript = bulletShellObj.GetComponent<BulletShell>();
            if (bulletShellScript != null)
            {
                bulletShellScript.AddEjectForce();
            }

            fireTimer = currentWeapon.fireRate;

            // update UI
            weaponDisplay.UpdateAmmoDisplay(currentWeapon, (int)currentAmmo);
        }
        else if (currentAmmo <= 0 && !isReloading)
        {
            StartReload();
        }
    }

    public override void StartReload()
    {
        // if there still ammo in the clip, empty it
        if (currentAmmo > 0)
        {
            currentAmmo = 0;

            // update UI
            weaponDisplay.UpdateAmmoDisplay(currentWeapon, (int)currentAmmo);
        }

        base.StartReload();
    }

    internal void StopFiring()
    {
        isFiring = false;
    }

    protected override void HandleReload()
    {
        if (isReloading)
        {
            if (reloadTimer < currentWeapon.reloadTime)
            {
                reloadTimer += Time.deltaTime;

                // update UI
                weaponDisplay.UpdateReloadDisplay(reloadTimer / currentWeapon.reloadTime);
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

    public void AddAmmo(int ammo)
    {
        currentAmmo += ammo;

        if (currentAmmo > currentWeapon.clipSize)
        {
            currentAmmo = currentWeapon.clipSize;
        }

        // update UI
        weaponDisplay.UpdateAmmoDisplay(currentWeapon, (int)currentAmmo);
    }

    // not used for now because the player may exploit this to instant reload
    //public void ThrowWeapon()
    //{
    //    if(currentWeapon != defaultWeapon)
    //    {
    //        // create weapon pickup object
    //        GameObject weaponPickupObj = Instantiate(weaponDropPrefab, transform.position, Quaternion.identity);

    //        WeaponPickUp weaponPickupScript = weaponPickupObj.GetComponent<WeaponPickUp>();
    //        if (weaponPickupScript != null)
    //        {
    //            weaponPickupScript.Init(currentWeapon);
    //        }

    //        // switch to default weapon
    //        SwitchWeapon(defaultWeapon);
    //    }
    //}
}