using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : CharacterStats
{
    [Header("References")]
    private GameMaster gameMaster;
    private HUDManager hudManager;

    [SerializeField] private bool invincible = false;

    protected override void Start()
    {
        base.Start();

        gameMaster = ServiceLocater.GetService<GameMaster>();
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

        // end of game
        StartCoroutine(DeathTransition());
    }

    private IEnumerator DeathTransition()
    {
        // pause the game by setting the time scale to 0
        Time.timeScale = 0.1f;

        // wait for a few seconds before restarting the game
        yield return new WaitForSecondsRealtime(3f);

        gameMaster.ReloadScene();
    }
}
