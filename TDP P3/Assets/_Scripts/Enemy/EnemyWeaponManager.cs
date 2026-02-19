using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWeaponManager : WeaponManager
{
    [Header("References")]
    private EnemyStats enemyStats;

    [SerializeField] private WeaponData _weaponData;

    public WeaponData weaponData
    {
        get => _weaponData; set => _weaponData = value;
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        enemyStats = GetComponent<EnemyStats>();
    }

    protected override void Start()
    {
        base.Start();

        currentWeapon = _weaponData;

        // reset fire rate
        fireTimer = 0f;

        // reset reloading
        isReloading = false;
        reloadTimer = 0f;

        currentAmmo = currentWeapon.clipSize;

        enemyStats.rotationMultiplier = currentWeapon.rotationMultiplier;
        enemyStats.dashPowerMultiplier = currentWeapon.dashPowerMultiplier;
    }

    private void Update()
    {
        HandleFireRate();
        HandleSpreadCooldown();
        HandleReload();
    }
}