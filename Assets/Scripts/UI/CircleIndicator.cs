using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleIndicator : MonoBehaviour
{
    public static Vector3 defaultRotate = new Vector3(0, 0, 30);

    public static bool useRelativeTime = true;

    public Vector3 rotationVector;
    public float x
    {
        get
        {
            return this.transform.position.x;
        }
        set
        {
            this.transform.position = new Vector3(value, y, z);
        }
    }
    public float y
    {
        get
        {
            return this.transform.position.y;
        }
        set
        {
            this.transform.position = new Vector3(x, value, z);
        }
    }
    public float z
    {
        get
        {
            return this.transform.position.z;
        }
        set
        {
            this.transform.position = new Vector3(x, y, value);
        }
    }

    private void Awake()
    {
        rotationVector = defaultRotate;
    }
    void Update()
    {
        if (useRelativeTime)
        {
            transform.Rotate(rotationVector * Time.deltaTime);
            return;
        }

        transform.Rotate(rotationVector);
    }
}
