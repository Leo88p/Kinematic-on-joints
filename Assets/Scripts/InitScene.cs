
using System.Collections.Generic;
using UnityEngine;

public class InitScene : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    Vector3 desiredScale = new Vector3(0.5f, 0.5f, 0.5f);
    static int segmentsAmount = 8;
    public static float height = 1.0f;
    public static List<Capsule> segments = new();
    public static bool hasCollided = false;
    void Start()
    {

        Vector3 segmentPosition = Vector3.zero;
        GameObject segment = Resources.Load<GameObject>("prefabs/segment");
        GameObject lastSegment = null;
        for (int i = 0; i < segmentsAmount; i++)
        {
            GameObject newSegment = Instantiate(segment, segmentPosition, Quaternion.identity);
            newSegment.transform.localScale = desiredScale;
            if (i > 0)
            {
                newSegment.GetComponent<Capsule>().predecessor = lastSegment;
            }
            segments.Add(newSegment.GetComponent<Capsule>());
            newSegment.GetComponent<Capsule>().segmentIndex = i;
            lastSegment = newSegment;
            segmentPosition.y += height;
        }
        segments.Reverse();

    }

    // Update is called once per frame
    void Update()
    {
    }
}
