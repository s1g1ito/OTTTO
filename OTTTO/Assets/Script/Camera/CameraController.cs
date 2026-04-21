using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public float mouseSensitivity = 80f;

    private InputAction lookAction;
    private float yaw = 0f;   // 左右回転

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
        Vector2 mouseDelta = lookAction.ReadValue<Vector2>() * mouseSensitivity * Time.deltaTime;

        // ★ Player を回さず、カメラだけ左右に回す
        yaw += mouseDelta.x;

        // ★ 上下を使わないなら pitch は更新しない
        // pitch -= mouseDelta.y;
        // pitch = Mathf.Clamp(pitch, -80f, 80f);

        transform.localRotation = Quaternion.Euler(0f, yaw, 0f);
    }
}
