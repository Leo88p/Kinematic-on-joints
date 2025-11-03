using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class Capsule : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public int pushDirection = 0;
    private float rotationSpeed = 20;
    private float pushImpact = 30;
    public GameObject predecessor;
    private Capsule predecessorCapsule;
    private float followSpeed = 10f;
    public int segmentIndex;
    public Vector3 lastStablePosition;
    public Quaternion lastStableLocalRotation;
    public Rigidbody ownRigibody;

    private Quaternion localRotation;

    void Start()
    {
        localRotation = Quaternion.identity;
        if (predecessor != null)
        {
            predecessorCapsule = predecessor.GetComponent<Capsule>();
        }
        ownRigibody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (!InitScene.hasCollided)
        {
            lastStablePosition = transform.position;
            lastStableLocalRotation = localRotation;
        }
        HandleRotation();
        HandlePosition();
    }

    private void HandleRotation()
    {
        if (pushDirection != 0)
        {
            Vector3 directionToObject = transform.position - Camera.main.transform.position;
            Vector3 perpendicular = Vector3.Cross(directionToObject, Vector3.up).normalized;
            float angle = pushImpact * pushDirection * Time.deltaTime;
            Quaternion rotationIncrement = Quaternion.AngleAxis(angle, perpendicular);
            localRotation = rotationIncrement * localRotation;
        }
        Quaternion targetRotation = localRotation;
        if (predecessor != null)
        {
            targetRotation = localRotation * predecessor.transform.rotation;
        }
        ownRigibody.MoveRotation(Quaternion.RotateTowards(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        ));
    }

    private void HandlePosition()
    {
        if (predecessorCapsule != null)
        {
            Vector3 targetPosition = predecessorCapsule.ownRigibody.position + (predecessor.transform.up * InitScene.height);
            ownRigibody.MovePosition(Vector3.Lerp(ownRigibody.position, targetPosition, followSpeed * Time.deltaTime));
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
        if (InitScene.hasCollided)
        {
            pushDirection = 0;
        }
    }

    public void OnPointerUp(PointerEventData pointerEventData)
    {
        pushDirection = 0;
    }

    void OnCollisionEnter(Collision collision)
    {
        InitScene.hasCollided = true;
        // Reset ALL segments to last stable state ONCE
        foreach (var s in InitScene.segments)
        {
            s.pushDirection = 0;
            s.ownRigibody.MovePosition(s.lastStablePosition);
            s.localRotation = s.lastStableLocalRotation;
        }
        InitScene.hasCollided = false;
    }
}