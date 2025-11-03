using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMover : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 60f; // Degrees per second

    private CameraControls controls;
    private Vector2 moveInput;
    private float rotateInput;

    private void Awake()
    {
        controls = new CameraControls();

        // Move bindings
        controls.Camera.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Camera.Move.canceled += ctx => moveInput = Vector2.zero;

        // Rotate bindings (Q/E mapped to X axis)
        controls.Camera.Rotate.performed += ctx => rotateInput = ctx.ReadValue<float>();
        controls.Camera.Rotate.canceled += ctx => rotateInput = 0;
    }

    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();

    private void Update()
    {
        HandleMovement();
        HandleRotation();
    }

    private void HandleMovement()
    {
        // Get forward/right directions (ignoring Y-axis)
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        // Calculate movement
        Vector3 move = (forward * moveInput.y + right * moveInput.x) * moveSpeed * Time.deltaTime;
        transform.position += move;
    }

    private void HandleRotation()
    {
        // Q = negative rotation (left), E = positive rotation (right)
        float rotation = rotateInput * rotationSpeed * Time.deltaTime;
        transform.Rotate(Vector3.up, rotation);
    }
}