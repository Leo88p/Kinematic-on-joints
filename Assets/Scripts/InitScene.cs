using System.Collections.Generic;
using UnityEngine;

public class InitScene : MonoBehaviour
{
    Vector3 desiredScale = new Vector3(0.5f, 0.5f, 0.5f);
    static int segmentsAmount = 8;
    public static float height = 1.0f;
    public static List<Capsule> segments = new();
    public static bool hasCollided = false;
    public bool isInverse = false;
    public GameObject target;
    private GameObject lastSegment;
    void Start()
    {

        Vector3 segmentPosition = Vector3.zero;
        GameObject segment = Resources.Load<GameObject>("prefabs/segment");
        lastSegment = null;
        for (int i = 0; i < segmentsAmount; i++)
        {
            GameObject newSegment = Instantiate(segment, segmentPosition, Quaternion.identity);
            newSegment.transform.localScale = desiredScale;
            Capsule newCapsule = newSegment.GetComponent<Capsule>();
            if (i > 0)
            {
                newCapsule.predecessor = lastSegment;
            }
            segmentPosition.y += 1;
            segments.Add(newCapsule);
            newCapsule.segmentIndex = i;
            if (i % 2 == 0)
            {
                newCapsule.rotationAxis = RotationAxis.Forward;
            }
            else
            {
                newCapsule.rotationAxis = RotationAxis.Right;
            }
            if (isInverse)
            {
                Destroy(newSegment.GetComponent<CapsuleCollider>());
            }
            lastSegment = newSegment;
        }
        if (isInverse && target != null)
        {
            target.transform.position = segmentPosition;
        }
    }
    private float Distance()
    {
        return Vector3.Distance(lastSegment.transform.position + lastSegment.transform.up * height, target.transform.position);
    }
    private void FixedUpdate()
    {
        if (!isInverse)
        {
            return;
        }
        float previousOptimizedDistance = float.MaxValue;
        while (Distance() > 0.005)
        {
            for (int i = 0; i < segmentsAmount; i++)
            {
                float angle = segments[i].LocalAngle;
                float currentDistance;
                float previousDistance = float.MaxValue;
                int currentDirection = 1;
                int changedDirectionCount = 0;
                do
                {
                    segments[i].LocalAngle = angle;
                    for (int j = i; j < segmentsAmount; j++)
                    {
                        segments[j].HandleRotation();
                        segments[j].HandlePosition();
                    }
                    currentDistance = Distance();
                    if (currentDistance < previousDistance)
                    {
                        previousDistance = currentDistance;
                        changedDirectionCount = 0;
                    }
                    else
                    {
                        currentDirection *= -1;
                        angle += currentDirection;
                        changedDirectionCount += 1;
                    }
                    angle += currentDirection;
                } while (changedDirectionCount < 2);
            }
            if (previousOptimizedDistance - Distance() < 0.005)
            {
                break;
            }
            else
            {
                previousOptimizedDistance = Distance();
            }
        }
    }
}
