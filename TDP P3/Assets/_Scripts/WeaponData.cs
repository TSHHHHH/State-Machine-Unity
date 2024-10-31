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

  public float damage;
  public float fireRate;
  public float bulletSpeed;
  public float range;
  public float reloadTime;
  public int clipSize;
  public float fireAngle;

  public bool isAutomatic;

  public float rotationMultiplier = 1f;
  public float dashPowerMultiplier = 1f;
}
