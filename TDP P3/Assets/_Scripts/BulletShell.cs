using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BulletShell : MonoBehaviour
{
    [SerializeField] private float ejectAngle;
    [SerializeField] private float shellSpeed;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void AddEjectForce()
    {
        // get a random small angle for the shell to eject
        float shellEjectAngle = Random.Range(-ejectAngle, ejectAngle);

        Vector3 shellDir = transform.right + new Vector3(0, 0, shellEjectAngle);

        rb.AddForce(shellDir * shellSpeed, ForceMode2D.Impulse);

        // get a random angle for the shell to rotate
        float shellAngle = Random.Range(-0.01f, 0.01f);

        rb.AddTorque(shellAngle, ForceMode2D.Impulse);
    }
}
