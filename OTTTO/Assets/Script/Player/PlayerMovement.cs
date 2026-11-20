using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // 箱を持っていない時の速度
    public float moveSpeed = 7f;

    // 箱を持っている時の速度
    public float holdingMoveSpeed = 3f;

    // 操作反転フラグ（新規追加）
    public bool isMoveReversed = false;
    // 操作反転が終了する時刻
    private float reverseMoveUntil = 0f;

    private Rigidbody rb;
    private InputAction moveAction;

    Animator animator;

    // 操作反転を開始・延長する
    public void ReverseMove(float duration)
    {
        isMoveReversed = true;

        reverseMoveUntil = Mathf.Max(
            reverseMoveUntil,
            Time.time + duration
        );
    }
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
        // 操作反転の終了判定
        if (isMoveReversed && Time.time >= reverseMoveUntil)
        {
            isMoveReversed = false;
        }

        // 入力取得
        Vector2 input = moveAction.ReadValue<Vector2>();

        // 入力の強さをAnimatorへ送る
        float speed = input.magnitude;

        animator.SetFloat("Speed", speed);
    }

    void FixedUpdate()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();

        // 操作反転処理（新規追加）
        if (isMoveReversed)
        {
            input = -input;
        }

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