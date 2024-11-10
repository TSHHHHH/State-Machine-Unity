using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [Header("References")]
    private PlayerLocomotion playerLocomotion;
    private PlayerWeaponManager playerWeaponManager;

    [Header("Player Movement")]
    public float horizontal;
    public float vertical;
    public Vector2 movementInput;

    [Header("Mouse Movement")]
    public float mouseX;
    public float mouseY;
    public Vector2 mouseInput;

    [Header("Input Flags")]
    private bool leftMouse_input;
    private PlayerInputActions inputActions;

    [Header("Interact")]
    [SerializeField] private EventChannel interactEventChannel;

    private void Awake()
    {
        playerLocomotion = GetComponent<PlayerLocomotion>();
        playerWeaponManager = GetComponent<PlayerWeaponManager>();
    }

    private void OnEnable()
    {
        if (inputActions == null)
        {
            inputActions = new PlayerInputActions();

            inputActions.Movement.WASD.performed += ctx => movementInput = ctx.ReadValue<Vector2>();

            inputActions.Actions.Aim.performed += ctx => mouseInput = ctx.ReadValue<Vector2>();

            inputActions.Actions.Shoot.started += ctx => leftMouse_input = true;
            inputActions.Actions.Shoot.canceled += ctx => leftMouse_input = false;

            inputActions.Actions.Dash.performed += ctx => Dash();

            inputActions.Actions.Interact.performed += ctx => Interact();
            inputActions.Actions.ThrowWeapon.performed += ctx => ThrowWeapon();
        }

        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    public void TickInput(float dt)
    {
        MoveInput();
        HandleFireInput();
    }

    private void MoveInput()
    {
        horizontal = movementInput.x;
        vertical = movementInput.y;

        mouseX = mouseInput.x;
        mouseY = mouseInput.y;
    }

    private void HandleFireInput()
    {
        if (leftMouse_input)
        {
            playerWeaponManager.Fire();
        }
        else
        {
            playerWeaponManager.StopFiring();
        }
    }

    private void Dash()
    {
        playerLocomotion.HandleDash();
    }

    private void Interact()
    {
        interactEventChannel.Invoke(new Empty());
    }

    private void ThrowWeapon()
    {
        // playerWeaponManager.ThrowWeapon();
    }
}
