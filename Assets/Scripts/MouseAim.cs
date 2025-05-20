using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The MouseAim class handles mouse look functionality for a first-person player controller.
// It allows the player to look around using mouse movement with a specified sensitivity.
public class MouseAim : MonoBehaviour
{
    // Serialized fields to adjust in the Unity Editor
    [SerializeField] private float mouseSensitivity = 100f; // Sensitivity of mouse movement
    [SerializeField] private Transform playerBody; // Reference to the player's body transform

    // Private variable to keep track of the vertical rotation
    private float xRotation = 0f;

    // Awake is called when the script instance is being loaded
    void Awake()
    {
        // Lock the cursor to the center of the screen and make it invisible
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        // Get mouse movement input
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        // Adjust the vertical rotation based on mouse Y input and clamp it to prevent over-rotation
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Apply the vertical rotation to the camera (this transform)
        transform.localRotation = Quaternion.Euler(xRotation, 0f, transform.localRotation.z);

        // Rotate the player's body horizontally based on mouse X input
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
