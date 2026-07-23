using UnityEngine;
using UnityEngine.InputSystem;

public class Camera : MonoBehaviour
{
    public float sensitivity = 0.2f;
    public Transform cameraTransform;

    private float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        float mouseX = mouseDelta.x * sensitivity;
        float mouseY = mouseDelta.y * sensitivity;

        // ç∂âEâÒì]
        transform.Rotate(Vector3.up * mouseX);

        // è„â∫âÒì]
        xRotation -= mouseY;

        // è„å¸Ç´êßå¿
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }
}