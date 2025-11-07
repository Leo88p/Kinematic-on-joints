using UnityEngine;

public class TargetMover : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    private TargetControls controls;
    private Vector3 moveInput;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        controls = new TargetControls();

        // Move bindings
        controls.Target.Move.performed += ctx => moveInput = ctx.ReadValue<Vector3>();
        controls.Target.Move.canceled += ctx => moveInput = Vector3.zero;
    }
    private void OnEnable() => controls.Enable();
    private void OnDisable() => controls.Disable();
    // Update is called once per frame
    void Update()
    {
        HandleMovement();
    }
    private void HandleMovement()
    {
        // Get forward/right directions (ignoring Y-axis)
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        Vector3 up = transform.up;
        forward.Normalize();
        right.Normalize();
        up.Normalize();

        // Calculate movement
        Vector3 move = (forward * moveInput.z + right * moveInput.x + up * moveInput.y) * moveSpeed * Time.deltaTime;
        transform.position += move;
    }

}
