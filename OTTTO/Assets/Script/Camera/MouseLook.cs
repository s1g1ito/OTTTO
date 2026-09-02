
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLook : MonoBehaviour
{
    // マウスの感度
    public float sensitivity = 100f;

    // プレイヤー本体のTransform
    // 左右の視点移動に使用する
    public Transform playerBody;

    // カメラの上下方向の回転角度
    float xRotation = 0f;

    void Start()
    {
        // マウスカーソルを画面中央に固定する
        // ゲーム中にカーソルが画面外へ出ないようにする
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // マウスの移動量を取得する
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();

        // マウスの左右の移動量を取得
        // 感度とフレーム時間を掛けて視点の移動量を調整する
        float mouseX = mouseDelta.x * sensitivity * Time.deltaTime;

        // マウスの上下の移動量を取得
        float mouseY = mouseDelta.y * sensitivity * Time.deltaTime;

        // マウスの上下移動によってカメラの回転角度を変更する
        // マウスを上に動かすと視点が上を向く
        xRotation -= mouseY;

        // カメラが上下90度以上回転しないように制限する
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // カメラ自身を上下方向に回転させる
        // X軸を使って上下の視点移動を行う
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // プレイヤー本体を左右に回転させる
        // Y軸を使って左右の視点移動を行う
        playerBody.Rotate(Vector3.up * mouseX);
    }
}

