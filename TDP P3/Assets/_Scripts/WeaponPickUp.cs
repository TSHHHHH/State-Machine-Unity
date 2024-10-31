using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickUp : MonoBehaviour
{
  [SerializeField] private Sprite weaponSprite;
  [SerializeField] private WeaponData weaponData;

  private bool interactable;

  private void OnEnable()
  {
    if(weaponData != null)
    {
      LoadWeaponIcon();
    }
  }

  public void Init(WeaponData weaponData)
  {
    this.weaponData = weaponData;

    LoadWeaponIcon();
  }

  private void LoadWeaponIcon()
  {
    weaponSprite = weaponData.weaponIcon;
  }

  private void OnTriggerEnter2D(Collider2D collision)
  {
    if(collision.CompareTag("Player"))
    {
      interactable = true;
    }
  }
}
