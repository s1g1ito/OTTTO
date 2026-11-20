using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
    // マウスの感度
    public float sensitivity = 100f;

    // 視点反転フラグ（新規追加）
    public bool isLookReversed = false;

    // プレイヤー本体のTransform
    public Transform playerBody;

    // カメラの上下方向の回転角度
    float xRotation = 0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        float sensitivityValue =
            PlayerPrefs.GetFloat("MouseSensitivity", 1.0f);

        sensitivity = 100f * sensitivityValue;

        Debug.Log("読み込んだマウス感度: " + sensitivity);
    }

    void Update()
    {
        // マウスの移動量を取得
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        // 視点反転処理（新規追加）
        if (isLookReversed)
        {
            mouseDelta = -mouseDelta;
        }

        float mouseX =
            mouseDelta.x * sensitivity * Time.deltaTime;

        float mouseY =
            mouseDelta.y * sensitivity * Time.deltaTime;

        xRotation -= mouseY;

        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation =
            Quaternion.Euler(xRotation, 0f, 0f);

        playerBody.Rotate(Vector3.up * mouseX);
    }
}