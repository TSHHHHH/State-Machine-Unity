using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHUDManager : MonoBehaviour
{
  [Header("References")]
  [SerializeField] private EnemyHealthBar healthBar;

  [Header("Display Vars")]
  [SerializeField] private float yOffset;

  private Transform followTransform;

  public void Init(Transform followTransform)
  {
    this.followTransform = followTransform;
  }

  private void Update()
  {
    if (followTransform != null)
    {
      Vector2 displayPos = followTransform.position + Vector3.up * yOffset;
    }
  }

  public void UpdateHealthDisplay(EnemyStats enemyStats)
  {
    healthBar.UpdateHealthDisplay(enemyStats);
  }
}
