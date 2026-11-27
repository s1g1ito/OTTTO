
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    // 箱を持っていない時の速度
    public float moveSpeed = 7f;

    // 箱を持っている時の速度
    public float holdingMoveSpeed = 3f;

    // 操作反転フラグ
    public bool isMoveReversed = false;

    private float reverseMoveUntil = 0f;

    private Rigidbody rb;
    private InputAction moveAction;
    private Animator animator;

    // 死亡フラグ
    private bool isDead = false;

    // リスタートまでの待ち時間
    public float deathRestartDelay = 5.8f;

    // =========================
    // 死亡時カメラ設定
    // =========================

    [Header("死亡時カメラズームアウト")]

    public Camera mainCamera;

    // 死亡時の視野角
    public float deathZoomOutFOV = 85f;

    // ズームアウトにかける時間
    public float deathZoomDuration = 2.5f;

    // 通常時の視野角を保存
    private float originalFOV;


    // =========================
    // 初期設定
    // =========================

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        moveAction = new InputAction(
            type: InputActionType.Value
        );

        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");
    }

    void Start()
    {
        animator = GetComponentInChildren<Animator>();

        // MainCameraを取得
        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        // 通常時の視野角を保存
        if (mainCamera != null)
        {
            originalFOV = mainCamera.fieldOfView;
        }

        isDead = false;
        isMoveReversed = false;

        if (animator != null)
        {
            animator.Rebind();
            animator.Update(0f);
            animator.SetFloat("Speed", 0f);
            animator.ResetTrigger("Death");
        }
    }

    void OnEnable()
    {
        moveAction.Enable();
    }

    void OnDisable()
    {
        moveAction.Disable();
    }


    // =========================
    // 操作反転
    // =========================

    public void ReverseMove(float duration)
    {
        if (isDead) return;

        isMoveReversed = true;

        reverseMoveUntil = Mathf.Max(
            reverseMoveUntil,
            Time.time + duration
        );
    }


    // =========================
    // 入力・アニメーション
    // =========================

    void Update()
    {
        if (isDead) return;

        // 操作反転の終了判定
        if (isMoveReversed && Time.time >= reverseMoveUntil)
        {
            isMoveReversed = false;
        }

        Vector2 input = moveAction.ReadValue<Vector2>();

        float speed = input.magnitude;

        if (animator != null)
        {
            animator.SetFloat("Speed", speed);
        }
    }


    // =========================
    // プレイヤー移動
    // =========================

    void FixedUpdate()
    {
        if (isDead) return;

        Vector2 input = moveAction.ReadValue<Vector2>();

        // 操作反転
        if (isMoveReversed)
        {
            input = -input;
        }

        Vector3 move =
            transform.forward * input.y +
            transform.right * input.x;

        float currentSpeed;

        // 箱を持っているか確認
        if (animator != null && animator.GetBool("IsHolding"))
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


    // =========================
    // Trigger接触判定
    // =========================

    private void OnTriggerEnter(Collider other)
    {
        CheckDeadlyObject(other.gameObject);
    }


    // =========================
    // 通常の衝突判定
    // =========================

    private void OnCollisionEnter(Collision collision)
    {
        CheckDeadlyObject(collision.gameObject);
    }


    // =========================
    // 死亡対象の確認
    // =========================

    private void CheckDeadlyObject(GameObject obj)
    {
        if (isDead) return;

        if (obj.CompareTag("Deadly"))
        {
            Die();
        }
    }


    // =========================
    // 死亡処理
    // =========================

    public void Die()
    {
        if (isDead) return;

        isDead = true;

        // プレイヤーの動きを停止
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        isMoveReversed = false;

        if (animator != null)
        {
            // Animatorを初期化
            animator.enabled = true;
            animator.speed = 1f;
            animator.Rebind();
            animator.Update(0f);

            // パラメータをリセット
            animator.SetFloat("Speed", 0f);
            animator.ResetTrigger("Death");

            // 死亡アニメーションを最初から再生
            animator.Play("Base Layer.Dying", 0, 0f);
            animator.Update(0f);

            Debug.Log("死亡アニメーション再生！");
        }
        else
        {
            Debug.LogError("Animatorが見つかりません！");
        }

        // 死亡時カメラズームアウト開始
        if (mainCamera != null)
        {
            StartCoroutine(DeathCameraZoomOut());
        }

        // リスタート処理開始
        StartCoroutine(RestartAfterDeath());
    }


    // =========================
    // 死亡時カメラズームアウト
    // =========================

    IEnumerator DeathCameraZoomOut()
    {
        float startFOV = mainCamera.fieldOfView;
        float elapsed = 0f;

        while (elapsed < deathZoomDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(
                elapsed / deathZoomDuration
            );

            // 徐々に変化するように補間
            t = Mathf.SmoothStep(0f, 1f, t);

            mainCamera.fieldOfView = Mathf.Lerp(
                startFOV,
                deathZoomOutFOV,
                t
            );

            yield return null;
        }

        // 最終値に設定
        mainCamera.fieldOfView = deathZoomOutFOV;
    }


    // =========================
    // 自動リスタート
    // =========================

    IEnumerator RestartAfterDeath()
    {
        yield return new WaitForSecondsRealtime(
            deathRestartDelay
        );

        // 現在のシーンを再読み込み
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }
}
