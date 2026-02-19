using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerWeaponDisplay : MonoBehaviour
{
    [SerializeField] private Image weaponFrame;
    [SerializeField] private Image weaponIcon;
    [SerializeField] private TextMeshProUGUI ammoDisplay;

    [Header("Weapon Spread Settings")]
    [SerializeField] private Color defaultColor;
    [SerializeField] private Color spreadColor;

    private void Awake()
    {
        ServiceLocater.RegisterService<PlayerWeaponDisplay>(this);
    }

    private void OnDisable()
    {
        ServiceLocater.UnregisterService<PlayerWeaponDisplay>();
    }

    public void UpdateWeaponDisplay(WeaponData weaponData)
    {
        weaponFrame.sprite = weaponData.weaponIcon;
        weaponIcon.sprite = weaponData.weaponIcon;
    }

    public void UpdateAmmoDisplay(WeaponData weaponData, int currentAmmoCnt)
    {
        ammoDisplay.text = $"{currentAmmoCnt}/{weaponData.clipSize}";
    }

    public void UpdateReloadDisplay(float percentage)
    {
        weaponIcon.fillAmount = percentage;
    }

    internal void UpdateSpreadFeedback(float spreadPercentage)
    {
        // Debug.Log(spreadPercentage);

        // set the color of the weapon icon based on the spread percentage
        weaponIcon.color = Color.Lerp(defaultColor, spreadColor, spreadPercentage);
    }
}