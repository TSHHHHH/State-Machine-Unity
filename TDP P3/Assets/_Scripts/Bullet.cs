using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
  [Header("References")]
  private Rigidbody2D rb;

  private float damage;

  private void Awake()
  {
    rb = GetComponent<Rigidbody2D>();
  }

  public void Init(float damage, float bulletSpeed)
  {
    this.damage = damage;

    rb.velocity = transform.up * bulletSpeed;
  }
}
