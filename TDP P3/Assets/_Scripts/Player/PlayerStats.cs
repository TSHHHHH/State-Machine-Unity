using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    [Header("References")]
    private HUDManager hudManager;

    [SerializeField] private bool invincible = false;

    protected override void Start()
    {
        base.Start();

        hudManager = ServiceLocater.GetService<HUDManager>();

        hudManager.UpdatePlayerHealthBar((float)currentHealth / maxHealth);
    }

    public override void HealFixedAmount(int healAmount)
    {
        currentHealth += healAmount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        hudManager.UpdatePlayerHealthBar((float)currentHealth / maxHealth);
    }

    public override void HealPercentage(float percentage)
    {
        currentHealth += (int)(maxHealth * percentage);

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        hudManager.UpdatePlayerHealthBar((float)currentHealth / maxHealth);
    }

    public override void TakeDamage(int damage)
    {
        if(invincible)
        {
            return;
        }

        currentHealth -= damage;

        if (currentHealth <= 0)
        {
            OnDeath();
        }

        hudManager.UpdatePlayerHealthBar((float)currentHealth / maxHealth);
    }

    protected override void OnDeath()
    {
        currentHealth = 0;

        // TODO: end of game
    }
}
