using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Weapon Data")]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public Sprite weaponIcon;

    public GameObject clipPrefab;

    public float damage;
    public float fireRate;
    public float bulletSpeed;
    public float range;

    [Header("Clip Info")]
    public int clipSize;
    public float reloadTime;

    [Header("Fire Spread")]
    public float maxSpreadAngle;
    public float spreadRate = 1f;

    public bool isAutomatic;

    public float rotationMultiplier = 1f;
    public float dashPowerMultiplier = 1f;
}