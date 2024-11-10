using UnityEngine;

public class DamageCollider : MonoBehaviour
{
    protected int damage;

    public void SetDamage(int damage)
    {
        this.damage = damage;
    }
}
