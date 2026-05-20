using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
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
        jumpAction = new InputAction(type: InputActionType.Button, binding: "<Keyboard>/space");
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
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // ジャンプ
        if (jumpAction.triggered && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        // 入力取得
        Vector2 input = moveAction.ReadValue<Vector2>();

        // 入力の強さ
        float speed = input.magnitude;

        // Animatorへ送る
        animator.SetFloat("Speed", speed);
    }

    void FixedUpdate()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();
        Vector3 move = transform.forward * input.y + transform.right * input.x;

        rb.MovePosition(rb.position + move * moveSpeed * Time.fixedDeltaTime);
    }
}