using Unity.Collections.LowLevel.Unsafe;
using UnityEditor.EditorTools;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [Header("References")]
    protected PoolManager poolManager;

    [SerializeField] protected Transform firePoint;

    [Header("Current Weapon Info")]
    protected WeaponData currentWeapon;

    protected bool isFiring = false;
    protected float currentSpread;
    protected float fireTimer;

    protected int currentAmmo;
    protected bool isReloading;
    protected float reloadTimer;

    protected virtual void Start()
    {
        poolManager = ServiceLocater.GetService<PoolManager>();
    }

    public virtual void Fire()
    {
        if (currentAmmo > 0 && fireTimer <= 0)
        {
            isFiring = true;

            --currentAmmo;

            Quaternion fireAngle = GetFireAngle();

            // create bullet object
            GameObject bulletObj = poolManager.SpawnFromPool("Enemy Bullet", firePoint.position, fireAngle);

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
        }
        else if (currentAmmo <= 0 && !isReloading)
        {
            StartReload();
        }
    }

    public virtual void StartReload()
    {
        isFiring = false;

        isReloading = true;

        reloadTimer = 0f;

        Quaternion fireAngle = GetFireAngle();

        // spawn a clip prefab
        GameObject clipObj = Instantiate(currentWeapon.clipPrefab, firePoint.position, fireAngle);

        EmptyClip emptyClip = clipObj.GetComponent<EmptyClip>();
        if (emptyClip != null)
        {
            emptyClip.AddEjectForce();
        }
    }

    protected Quaternion GetFireAngle()
    {
        if (currentSpread < currentWeapon.maxSpreadAngle)
        {
            currentSpread += currentWeapon.spreadRate;

            currentSpread = Mathf.Clamp(currentSpread, 0, currentWeapon.maxSpreadAngle);
        }

        float randomAngle = Random.Range(-currentSpread, currentSpread);

        Quaternion rotation = Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + randomAngle);

        return rotation;
    }

    protected virtual void HandleFireRate()
    {
        if (fireTimer > 0)
        {
            fireTimer -= Time.deltaTime;
        }
    }

    protected virtual void HandleSpreadCooldown()
    {
        if (!isFiring && currentSpread > 0)
        {
            currentSpread -= currentWeapon.spreadRate * Time.deltaTime;
        }
    }

    protected virtual void HandleReload()
    {
        if (isReloading)
        {
            if (reloadTimer < currentWeapon.reloadTime)
            {
                reloadTimer += Time.deltaTime;
            }
            else
            {
                isReloading = false;

                currentAmmo = currentWeapon.clipSize;
            }
        }
    }
}
