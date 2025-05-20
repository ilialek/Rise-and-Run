using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The RotateAround class makes the attached GameObject rotate around a specified target object at a given speed.
public class RotateAround : MonoBehaviour
{
    [SerializeField] private Transform target; // The target object to rotate around
    [SerializeField] private float speed; // The speed of rotation

    // Update is called once per frame
    void Update()
    {
        // Rotate the GameObject around the target object's position, around the up axis (y-axis), at the specified speed
        transform.RotateAround(target.transform.position, Vector3.up, speed * Time.deltaTime);
    }
}
