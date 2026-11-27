
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("移動設定")]
    public float moveSpeed = 7f;
    public float holdingMoveSpeed = 3f;

    [Header("操作反転")]
    public bool isMoveReversed = false;

    private float reverseMoveUntil = 0f;

    private Rigidbody rb;
    private InputAction moveAction;
    private Animator animator;
    private PlayerRespawn playerRespawn;

    // 死亡中かどうか
    private bool isDead = false;

    public bool IsDead => isDead;

    [Header("死亡アニメーション")]
    [SerializeField]
    private string deathStateName = "Base Layer.Dying";

    [Tooltip("死亡アニメーション終了後の追加待機時間")]
    [Min(0f)]
    public float extraWaitAfterDeathAnimation = 0f;

    [Tooltip("アニメーションが終了しない場合の最大待機時間")]
    [Min(1f)]
    public float maxDeathAnimationWait = 15f;

    [Header("死亡時カメラズームアウト")]
    public Camera mainCamera;

    public float deathZoomOutFOV = 85f;
    public float deathZoomDuration = 2.5f;

    private float originalFOV;
    private Coroutine cameraZoomCoroutine;

    // =========================
    // 初期設定
    // =========================

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.freezeRotation = true;
        }

        playerRespawn = GetComponent<PlayerRespawn>();

        moveAction = new InputAction(
            type: InputActionType.Value
        );

        moveAction.AddCompositeBinding("2DVector")
            .With("Up", "<Keyboard>/w")
            .With("Down", "<Keyboard>/s")
            .With("Left", "<Keyboard>/a")
            .With("Right", "<Keyboard>/d");
    }

    private void Start()
    {
        animator = GetComponentInChildren<Animator>(true);

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }

        if (mainCamera != null)
        {
            originalFOV = mainCamera.fieldOfView;
        }

        ResetAnimation();

        isDead = false;
        isMoveReversed = false;
    }

    private void OnEnable()
    {
        if (moveAction != null)
        {
            moveAction.Enable();
        }
    }

    private void OnDisable()
    {
        if (moveAction != null)
        {
            moveAction.Disable();
        }
    }

    private void OnDestroy()
    {
        if (moveAction != null)
        {
            moveAction.Dispose();
            moveAction = null;
        }
    }

    // =========================
    // 操作反転
    // =========================

    public void ReverseMove(float duration)
    {
        if (isDead)
        {
            return;
        }

        isMoveReversed = true;

        reverseMoveUntil = Mathf.Max(
            reverseMoveUntil,
            Time.time + duration
        );
    }

    // =========================
    // 入力・通常アニメーション
    // =========================

    private void Update()
    {
        if (isDead)
        {
            return;
        }

        if (isMoveReversed &&
            Time.time >= reverseMoveUntil)
        {
            isMoveReversed = false;
        }

        Vector2 input =
            moveAction.ReadValue<Vector2>();

        float speed = input.magnitude;

        if (animator != null)
        {
            animator.SetFloat("Speed", speed);
        }
    }

    // =========================
    // プレイヤー移動
    // =========================

    private void FixedUpdate()
    {
        if (isDead || rb == null)
        {
            return;
        }

        Vector2 input =
            moveAction.ReadValue<Vector2>();

        if (isMoveReversed)
        {
            input = -input;
        }

        Vector3 move =
            transform.forward * input.y +
            transform.right * input.x;

        float currentSpeed = moveSpeed;

        if (animator != null &&
            animator.GetBool("IsHolding"))
        {
            currentSpeed = holdingMoveSpeed;
        }

        rb.MovePosition(
            rb.position +
            move * currentSpeed * Time.fixedDeltaTime
        );
    }

    // =========================
    // Trigger接触
    // =========================

    private void OnTriggerEnter(Collider other)
    {
        CheckDeadlyObject(other.gameObject);
    }

    // =========================
    // 通常の衝突
    // =========================

    private void OnCollisionEnter(Collision collision)
    {
        CheckDeadlyObject(collision.gameObject);
    }

    // =========================
    // 死亡対象の判定
    // =========================

    private void CheckDeadlyObject(GameObject obj)
    {
        if (isDead)
        {
            return;
        }

        if (obj.CompareTag("Deadly"))
        {
            Die();
        }
    }

    // =========================
    // 死亡開始
    // =========================

    public void Die()
    {
        if (isDead)
        {
            return;
        }

        // リスポーン直後の連続死亡を防ぐ
        if (playerRespawn != null &&
            !playerRespawn.CanStartDeath)
        {
            return;
        }

        isDead = true;
        isMoveReversed = false;
        reverseMoveUntil = 0f;

        Debug.Log("プレイヤー死亡！");

        // 移動速度をリセット
        StopRigidbody();

        bool animationStarted = false;

        // =========================
        // 死亡アニメーション再生
        // =========================

        if (animator != null)
        {
            animator.enabled = true;
            animator.speed = 1f;

            animator.Rebind();
            animator.Update(0f);

            animator.SetFloat("Speed", 0f);
            animator.ResetTrigger("Death");

            int deathHash =
                Animator.StringToHash(deathStateName);

            if (animator.HasState(0, deathHash))
            {
                animator.Play(
                    deathStateName,
                    0,
                    0f
                );

                animator.Update(0f);

                animationStarted = true;

                Debug.Log(
                    "死亡アニメーション再生開始！"
                );
            }
            else
            {
                Debug.LogError(
                    "Animatorに死亡ステートがありません：" +
                    deathStateName
                );
            }
        }
        else
        {
            Debug.LogError(
                "Animatorが見つかりません！"
            );
        }

        // =========================
        // カメラズームアウト
        // =========================

        if (mainCamera != null)
        {
            cameraZoomCoroutine =
                StartCoroutine(DeathCameraZoomOut());
        }

        // =========================
        // アニメーション終了後にリスポーン
        // =========================

        StartCoroutine(
            WaitForDeathAndRespawn(animationStarted)
        );
    }

    // =========================
    // Rigidbody停止
    // =========================

    private void StopRigidbody()
    {
        if (rb == null || rb.isKinematic)
        {
            return;
        }

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    // =========================
    // アニメーション終了待機
    // =========================

    private IEnumerator WaitForDeathAndRespawn(
        bool animationStarted)
    {
        if (animationStarted)
        {
            int deathHash =
                Animator.StringToHash(deathStateName);

            float startTime = Time.unscaledTime;
            float expectedDuration = 0f;

            bool enteredDeathState = false;
            bool finished = false;

            while (Time.unscaledTime - startTime <
                   maxDeathAnimationWait)
            {
                if (animator == null ||
                    !animator.isActiveAndEnabled)
                {
                    break;
                }

                AnimatorStateInfo state =
                    animator.GetCurrentAnimatorStateInfo(0);

                if (state.fullPathHash == deathHash)
                {
                    enteredDeathState = true;

                    // ステートの再生時間を記録
                    expectedDuration = Mathf.Max(
                        expectedDuration,
                        state.length /
                        Mathf.Max(0.01f, animator.speed)
                    );

                    // 1回分のアニメーションが終了
                    if (state.normalizedTime >= 1f)
                    {
                        finished = true;
                        break;
                    }
                }
                else if (enteredDeathState)
                {
                    // 別ステートに遷移した場合も
                    // 元の死亡アニメーションの長さまでは待つ
                    float elapsed =
                        Time.unscaledTime - startTime;

                    float remaining =
                        expectedDuration - elapsed;

                    if (remaining > 0f)
                    {
                        yield return new WaitForSecondsRealtime(
                            remaining
                        );
                    }

                    finished = true;
                    break;
                }

                yield return null;
            }

            if (!finished)
            {
                Debug.LogWarning(
                    "死亡アニメーションの終了を確認できませんでした。" +
                    "AnimatorのDyingステートを確認してください。"
                );
            }
        }
        else
        {
            // ステート未設定時の安全用待機
            yield return new WaitForSecondsRealtime(1f);
        }

        // アニメーション終了後、必要なら追加待機
        if (extraWaitAfterDeathAnimation > 0f)
        {
            yield return new WaitForSecondsRealtime(
                extraWaitAfterDeathAnimation
            );
        }

        // =========================
        // リスポーン処理
        // =========================

        if (playerRespawn == null)
        {
            playerRespawn = GetComponent<PlayerRespawn>();
        }

        if (playerRespawn == null)
        {
            Debug.LogError(
                "PlayerRespawnが付いていません！"
            );

            yield break;
        }

        // 迷路とプレイヤーの位置をリセット
        playerRespawn.FinishRespawn();

        // カメラを元に戻す
        RestoreCamera();

        // Animatorを通常状態へ戻す
        ResetAnimation();

        StopRigidbody();

        // 再び操作を有効化
        isDead = false;

        Debug.Log(
            "死亡アニメーション終了・リスポーン完了！"
        );
    }

    // =========================
    // 死亡時カメラズームアウト
    // =========================

    private IEnumerator DeathCameraZoomOut()
    {
        float startFOV = mainCamera.fieldOfView;
        float elapsed = 0f;

        if (deathZoomDuration <= 0f)
        {
            mainCamera.fieldOfView = deathZoomOutFOV;
            yield break;
        }

        while (elapsed < deathZoomDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(
                elapsed / deathZoomDuration
            );

            t = Mathf.SmoothStep(0f, 1f, t);

            mainCamera.fieldOfView = Mathf.Lerp(
                startFOV,
                deathZoomOutFOV,
                t
            );

            yield return null;
        }

        mainCamera.fieldOfView = deathZoomOutFOV;
        cameraZoomCoroutine = null;
    }

    // =========================
    // カメラを元に戻す
    // =========================

    private void RestoreCamera()
    {
        if (cameraZoomCoroutine != null)
        {
            StopCoroutine(cameraZoomCoroutine);
            cameraZoomCoroutine = null;
        }

        if (mainCamera != null)
        {
            mainCamera.fieldOfView = originalFOV;
        }
    }

    // =========================
    // Animatorリセット
    // =========================

    private void ResetAnimation()
    {
        if (animator == null)
        {
            return;
        }

        animator.enabled = true;
        animator.speed = 1f;

        animator.Rebind();
        animator.Update(0f);

        animator.SetFloat("Speed", 0f);
        animator.ResetTrigger("Death");
    }
}
