using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageCollider : MonoBehaviour
{
  [SerializeField] private GameObject impactFX;

  private void OnTriggerEnter2D(Collider2D collision)
  {
    if (impactFX != null)
    {
      Instantiate(impactFX, transform.position, Quaternion.identity);
    }

    Destroy(gameObject);
  }
}
