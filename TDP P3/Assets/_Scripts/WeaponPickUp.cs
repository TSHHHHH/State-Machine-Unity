using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickUp : MonoBehaviour
{
    [SerializeField] private WeaponData weaponData;

    [Header("Display Settings")]
    [SerializeField] private SpriteRenderer weaponSpriteRender;
    [SerializeField] private GameObject interactIcon;

    [Header("Events")]
    [SerializeField] private WeaponDataEventChannel weaponSwapEvent;

    private bool interactable;

    private void OnEnable()
    {
        if(weaponData != null)
        {
            LoadWeaponIcon();

            // disable interact icon by default
            interactIcon.SetActive(false);
        }
    }

    public void Init(WeaponData weaponData)
    {
        this.weaponData = weaponData;

        LoadWeaponIcon();
    }

    private void LoadWeaponIcon()
    {
        weaponSpriteRender.sprite = weaponData.weaponIcon;
    }

    public void OnPlayerPickUp()
    {
        if (interactable)
        {
            // send weapon swap event
            weaponSwapEvent.Invoke(weaponData);

            // destroy this object
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            interactable = true;

            interactIcon.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            interactable = false;

            interactIcon.SetActive(false);
        }
    }
}
