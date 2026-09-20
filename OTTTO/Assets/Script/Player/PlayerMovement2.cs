using UnityEngine;

public class PlayerMovement2 : MonoBehaviour
{
    [Header("à⁄ìÆê›íË")]
    public float speed = 5f;
    public float groundCheckDistance = 1.1f;

    [Header("éãì_à⁄ìÆê›íË")]
    public float mouseSensitivity = 150f;
    public Transform cameraTransform;

    float xRotation = 0f;

    public void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Update()
    {
        HandleMouseLook();
        HandleMovement();
    }

    public void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);
    }

    public void HandleMovement()
    {
        RaycastHit hit;
        bool isGrounded = Physics.Raycast(transform.position, Vector3.down, out hit, groundCheckDistance);

        if (!isGrounded || hit.collider.tag != "Ground")
        {
            return;
        }

        float h = Input.GetAxis("Horizotal");
        float v = Input.GetAxis("Vertical");

        Vector3 move = transform.right * h + transform.forward * v;

        transform.Translate(move * speed * Time.deltaTime, Space.World);
    }
}
