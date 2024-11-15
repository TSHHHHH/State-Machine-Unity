using UnityEngine;

public class HUDManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerHealthBar playerHealthBar;

    private void Awake()
    {
        ServiceLocater.RegisterService(this);
    }

    private void OnDisable()
    {
        ServiceLocater.UnregisterService<HUDManager>();
    }

    public void UpdatePlayerHealthBar(float healthPercent)
    {
        playerHealthBar.UpdatePlayerHealthBar(healthPercent);
    }
}
