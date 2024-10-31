using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DashCDDisplay : MonoBehaviour
{
  [Header("References")]
  [SerializeField] private PlayerStats playerStats;

  [SerializeField] private Image cdFill;
  [SerializeField] private Color cdColor;
  [SerializeField] private Color readyColor;

  private void Update()
  {
    HandleCDDisplay();
  }

  private void HandleCDDisplay()
  {
    if(playerStats.dashTimer < playerStats.totalDashCD)
    {
      cdFill.fillAmount = playerStats.dashTimer / playerStats.dashCooldown;
      cdFill.color = cdColor;
    }
    else
    {
      cdFill.fillAmount = 1;
      cdFill.color = readyColor;
    }
  }
}
