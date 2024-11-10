using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterStats : MonoBehaviour
{
  [Header("Health")]
  public int maxHealth = 100;
  public int currentHealth;
  public bool isDead => currentHealth <= 0;

  [Header("Movement")]
  public float moveSpeed = 5f;
  public float rotationSpeed = 10f;
  public float rotationMultiplier = 1f;

  [Header("Dash Settings")]
  public float dashPower = 10f;
  public float dashPowerMultiplier = 1f;
  public float dashDuration = 0.15f;
  public float dashCooldown = 2f;
  public float totalDashCD => dashCooldown + dashDuration;
  public float dashTimer = 0f;
  public bool isDashing = false;

  protected virtual void Start()
  {
    currentHealth = maxHealth;

    dashTimer = totalDashCD;
  }

  protected virtual void Update()
  {
    if(dashTimer < totalDashCD)
    {
      dashTimer += Time.deltaTime;
    }
  }

  public void TriggerDash()
  {
    if(dashTimer < totalDashCD)
    {
      return;
    }

    isDashing = true;

    // the dash total cooldown is the sum of the dash cooldown and the dash duration
    dashTimer = 0;
  }

  public abstract void TakeDamage(int damage);

  protected abstract void OnDeath();
}
