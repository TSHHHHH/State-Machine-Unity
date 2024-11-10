using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStats
{
  public override void TakeDamage(int damage)
  {
    currentHealth -= damage;

    if (currentHealth <= 0)
    {
      OnDeath();
    }
  }

  protected override void OnDeath()
  {
    currentHealth = 0;

    // TODO: end of game
  }
}
