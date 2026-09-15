using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // 箱を持っていない時の速度
    public float moveSpeed = 7f;

    // 箱を持っている時の速度
    public float holdingMoveSpeed = 3f;

    private Rigidbody rb;
    private InputAction moveAction;

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

        // 移動入力
        moveAction = new InputAction(type: InputActionType.Value);

        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");
    }

    void OnEnable()
    {
        moveAction.Enable();
    }

    void OnDisable()
    {
        moveAction.Disable();
    }

    void Update()
    {
        // 入力取得
        Vector2 input = moveAction.ReadValue<Vector2>();

        // 入力の強さをAnimatorへ送る
        float speed = input.magnitude;

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
            currentSpeed = holdingMoveSpeed;
        }
        else
        {
            currentSpeed = moveSpeed;
        }

        rb.MovePosition(
            rb.position +
            move * currentSpeed * Time.fixedDeltaTime
        );
    }
}