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

    private bool isEmptyReload = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
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

            // play fire sound
            AudioClip fireSound = currentWeapon.fireSound;
            if (fireSound != null && audioSource != null)
            {
                audioSource.pitch = Random.Range(0.95f, 1.05f);
                audioSource.volume = Random.Range(0.8f, 1f);
                audioSource.PlayOneShot(fireSound);
            }

            // update UI
            weaponDisplay.UpdateAmmoDisplay(currentWeapon, (int)currentAmmo);
        }
        else if (currentAmmo <= 0 && !isReloading)
        {
            StartReload();
        }
    }

    public void UpdateSpreadFeedback()
    {
        // base on the spread angle, change the color of the weapon icon
        float spreadPercentage = currentSpread / currentWeapon.maxSpreadAngle;

        weaponDisplay.UpdateSpreadFeedback(spreadPercentage);
    }

    public override void StartReload()
    {
        // if reload with empty clip, notify the reload function to play additional rack sound effect after reload ends
        isEmptyReload = currentAmmo == 0;

        // if there still ammo in the clip, empty it
        if (currentAmmo > 0)
        {
            currentAmmo = 0;

            // update UI
            weaponDisplay.UpdateAmmoDisplay(currentWeapon, (int)currentAmmo);
        }

        // play reload sound
        AudioClip reloadSfx = currentWeapon.reloadSound_Start;
        if (reloadSfx != null && audioSource != null)
        {
            AudioSource.PlayClipAtPoint(reloadSfx, transform.position);
        }
        // play the end sound effect at the 10% mark of the reload time
        float reloadEndSfxDelay = currentWeapon.reloadTime * 0.9f;
        Invoke(nameof(PlayReloadEndSfx), reloadEndSfxDelay);

        base.StartReload();
    }

    private void PlayReloadEndSfx()
    {
        AudioClip reloadEndSfx = currentWeapon.reloadSound_End;
        if (reloadEndSfx != null && audioSource != null)
        {
            AudioSource.PlayClipAtPoint(reloadEndSfx, transform.position);
        }
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

                // play rack sound effect if it's an empty reload
                if (isEmptyReload)
                {
                    AudioClip rackSfx = currentWeapon.reloadSound_Rack;
                    if (rackSfx != null)
                    {
                        AudioSource.PlayClipAtPoint(rackSfx, transform.position);
                    }
                }
                // reset empty reload flag
                isEmptyReload = false;

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