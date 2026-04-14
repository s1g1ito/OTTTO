using UnityEngine;
using UnityEngine.InputSystem;

public class MoveCapsule : MonoBehaviour
{
    public float speed = 3f;
    public float jumpPower = 5f;
    public float groundCheckDistance = 1.1f; // ← 重要：Capsule の高さに合わせて調整

    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        var keyboard = Keyboard.current;

        Vector3 v = Vector3.zero;

        if (keyboard.upArrowKey.isPressed)
            v = transform.forward * speed;

        if (keyboard.downArrowKey.isPressed)
            v = -transform.forward * speed;

        if (keyboard.rightArrowKey.isPressed)
            v = transform.right * speed;

        if (keyboard.leftArrowKey.isPressed)
            v = -transform.right * speed;

        // XZ 方向の移動（Y は保持）
        rb.linearVelocity = new Vector3(v.x, rb.linearVelocity.y, v.z);

        // ★ 地面チェック（Raycast）
        bool isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);

        // ★ デバッグ用（Sceneビューに赤線が出る）
        Debug.DrawRay(transform.position, Vector3.down * groundCheckDistance, Color.red);

        // ★ 地面にいる時だけジャンプ
        if (isGrounded && keyboard.spaceKey.wasPressedThisFrame)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpPower, rb.linearVelocity.z);
        }
    }
}


