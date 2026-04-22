using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
    public Transform playerBody;
    public float sensitivity = 0.2f;

    private InputAction lookAction;
    private float xRotation = 0f;

    void Awake()
    {
        lookAction = new InputAction(type: InputActionType.Value);
        lookAction.AddBinding("<Mouse>/delta");
    }

    void OnEnable()
    {
        lookAction.Enable();
    }

    void OnDisable()
    {
        lookAction.Disable();
    }

    void Update()
    {
        Vector2 look = lookAction.ReadValue<Vector2>();

        float mouseX = look.x * sensitivity;
        float mouseY = look.y * sensitivity;

        // 上下（カメラ）
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // 左右（プレイヤー）
        playerBody.Rotate(Vector3.up * mouseX);
    }
}