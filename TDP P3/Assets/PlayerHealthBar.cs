using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField] private Image healthBarFill;

    internal void UpdatePlayerHealthBar(float healthPercent)
    {
        healthBarFill.fillAmount = healthPercent;
    }
}
