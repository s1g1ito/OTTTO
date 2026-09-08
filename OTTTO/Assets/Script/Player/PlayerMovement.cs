using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // 箱を持っていない時の速度
    public float moveSpeed = 7f;

    // 箱を持っている時の速度
    public float holdingMoveSpeed = 3f;

    public float jumpForce = 5f;
    public Transform groundCheck;
    public float groundDistance = 0.2f;
    public LayerMask groundMask;

    private Rigidbody rb;
    private InputAction moveAction;
    private InputAction jumpAction;

    private bool isGrounded;

    Animator animator;

    void Start()
    {
        // 子オブジェクトに付いているAnimatorを取得
        animator = GetComponentInChildren<Animator>();
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        moveAction = new InputAction(type: InputActionType.Value);

        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");

        // ジャンプ入力
        jumpAction = new InputAction(
            type: InputActionType.Button,
            binding: "<Keyboard>/space"
        );
    }

    void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();
    }

    void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
    }

    void Update()
    {
        // 接地判定
        isGrounded = Physics.CheckSphere(
            groundCheck.position,
            groundDistance,
            groundMask
        );

        // ジャンプ
        if (jumpAction.triggered && isGrounded)
        {
            rb.AddForce(
                Vector3.up * jumpForce,
                ForceMode.Impulse
            );
        }

        // 入力取得
        Vector2 input = moveAction.ReadValue<Vector2>();

        // 入力の強さ
        float speed = input.magnitude;

        // AnimatorへSpeedを送る
        animator.SetFloat("Speed", speed);
    }

    void FixedUpdate()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();

        Vector3 move =
            transform.forward * input.y +
            transform.right * input.x;

        // 箱を持っているかで速度を変更
        float currentSpeed;

        if (animator.GetBool("IsHolding"))
        {
            // 箱を持っている
            currentSpeed = holdingMoveSpeed;
        }
        else
        {
            // 箱を持っていない
            currentSpeed = moveSpeed;
        }

        rb.MovePosition(
            rb.position +
            move * currentSpeed * Time.fixedDeltaTime
        );
    }
}