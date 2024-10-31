using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BulletShell : MonoBehaviour
{
  [SerializeField] private float shellSpeed;

  private Rigidbody2D rb;

  private void Awake()
  {
    rb = GetComponent<Rigidbody2D>();
  }

  private void OnEnable()
  {
    rb.AddForce(transform.right * shellSpeed , ForceMode2D.Impulse);

    // get a random angle for the shell to rotate
    float shellAngle = Random.Range(-0.01f, 0.01f);

    rb.AddTorque(shellAngle, ForceMode2D.Impulse);
  }
}
