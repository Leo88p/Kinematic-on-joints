using UnityEngine;
using UnityEngine.EventSystems;

public enum RotationAxis
{
    Right,
    Forward
}

public class Capsule : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public int pushDirection = 0;
    private float pushImpact = 30;
    public GameObject predecessor;
    private Capsule predecessorCapsule;
    public int segmentIndex;
    public Rigidbody ownRigibody;
    public RotationAxis rotationAxis;
    private float localAngle = 0;
    public float LocalAngle { 
        get { return localAngle; }
        set {
            if (value < -90)
            {
                localAngle = -90;
            }
            else if (value > 90)
            {
                localAngle = 90;
            }
            else
            {
                localAngle = value;
            }
        }
    }

    void Start()
    {
        if (predecessor != null)
        {
            predecessorCapsule = predecessor.GetComponent<Capsule>();
        }
        ownRigibody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        HandleRotation();
        HandlePosition();
    }

    public void HandleRotation()
    {
        Vector3 axis = transform.position;
        switch (rotationAxis)
        {
            case RotationAxis.Right:
                axis = Vector3.forward;
                break;
            case RotationAxis.Forward:
                axis = Vector3.right;
                break;
        }
        if (pushDirection != 0)
        {
            LocalAngle += pushImpact * pushDirection * Time.deltaTime;
        }
        Quaternion targetRotation = Quaternion.AngleAxis(LocalAngle, axis);
        if (predecessor != null)
        {
            targetRotation = targetRotation * predecessor.transform.rotation;
        }
        transform.rotation = targetRotation;
    }

    public void HandlePosition()
    {
        if (predecessorCapsule != null)
        {
            Vector3 targetPosition = predecessor.transform.position + (predecessor.transform.up * InitScene.height);
            transform.position = targetPosition;
        }
    }
    // Detect collisions with other segments

    public void OnPointerDown(PointerEventData pointerEventData)
    {
        // Only allow input if not colliding
        switch (pointerEventData.button)
        {
            case PointerEventData.InputButton.Left:
                pushDirection = -1;
                break;
            case PointerEventData.InputButton.Right:
                pushDirection = 1;
                break;
        }
    }

    public void OnPointerUp(PointerEventData pointerEventData)
    {
        pushDirection = 0;
    }
}