using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
  [SerializeField] private Image healthFill;

  public void UpdateHealthDisplay(EnemyStats enemyStats)
  {
    healthFill.fillAmount = (float)enemyStats.currentHealth / enemyStats.maxHealth;
  }
}
