using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
  [Header("References")]
  private Rigidbody2D rb;
  private InputManager inputManager;
  private PlayerStats playerStats;

  private Vector2 moveDir;
  private Vector2 mouseDir;

  private void Awake()
  {
    rb = GetComponent<Rigidbody2D>();
    inputManager = GetComponent<InputManager>();
    playerStats = GetComponent<PlayerStats>();
  }

  public void HandleMovement()
  {
    moveDir = inputManager.movementInput;
    moveDir.Normalize();

    // move the player rigidbody
    rb.linearVelocity = moveDir * playerStats.moveSpeed;
  }

  public void HandleRotation(float dt)
  {
    // get mouse positions
    Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

    mouseDir = mousePos - rb.position;
    mouseDir.Normalize();

    // compute rotation to look at mouse
    float angle = Mathf.Atan2(mouseDir.y, mouseDir.x) * Mathf.Rad2Deg;
    Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle - 90f));

    // lerp to the rotation
    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, playerStats.rotationSpeed * playerStats.rotationMultiplier * dt);
  }

  public void HandleDash()
  {
    playerStats.TriggerDash();

    // apply a force to the player rigidbody
    rb.AddForce(moveDir * playerStats.moveSpeed * playerStats.dashPower * playerStats.dashPowerMultiplier, ForceMode2D.Impulse);

    Invoke(nameof(EndDash), playerStats.dashDuration);
  }

  private void EndDash()
  {
    playerStats.isDashing = false;
  }
}
