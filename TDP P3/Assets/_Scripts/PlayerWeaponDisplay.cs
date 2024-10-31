using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerWeaponDisplay : MonoBehaviour
{
  [SerializeField] private Image weaponIcon;
  [SerializeField] private TextMeshProUGUI ammoDisplay;

  private void Awake()
  {
    ServiceLocater.RegisterService<PlayerWeaponDisplay>(this);
  }

  public void UpdateWeaponDisplay()
  {

  }

  public void UpdateAmmoDisplay(WeaponData weaponData, int currentAmmoCnt)
  {
    ammoDisplay.text = $"{currentAmmoCnt}/{weaponData.clipSize}";
  }
}
