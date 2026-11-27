
using System.Collections;
using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [Header("リスポーン設定")]
    [SerializeField] private MazeGenerator mazeGenerator;

    [SerializeField] private float fallHeight = -5f;

    [Header("ミニマップ")]
    [SerializeField] private FogOfWar fogOfWar;

    [Header("連続死亡防止")]
    [SerializeField] private float respawnCooldown = 0.5f;

    private Rigidbody rb;
    private PlayerMovement playerMovement;

    private bool isInRespawnCooldown = false;

    public bool CanStartDeath =>
        !isInRespawnCooldown;

    // =========================
    // 初期設定
    // =========================

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        playerMovement =
            GetComponent<PlayerMovement>();
    }

    private void Start()
    {
        FindMazeGenerator();
    }

    // =========================
    // 落下による死亡
    // =========================

    private void Update()
    {
        if (isInRespawnCooldown)
        {
            return;
        }

        if (playerMovement != null &&
            playerMovement.IsDead)
        {
            return;
        }

        if (transform.position.y < fallHeight)
        {
            Respawn();
        }
    }

    // =========================
    // 通常の接触判定
    // =========================

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Gimick") ||
            collision.gameObject.CompareTag("Enemy"))
        {
            Respawn();
        }
    }

    // =========================
    // Trigger接触判定
    // =========================

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Gimick") ||
            other.CompareTag("Enemy"))
        {
            Respawn();
        }
    }

    // =========================
    // MazeGenerator取得
    // =========================

    private void FindMazeGenerator()
    {
        if (mazeGenerator == null)
        {
            mazeGenerator =
                FindFirstObjectByType<MazeGenerator>();
        }
    }

    // =========================
    // 死亡処理を開始する
    // =========================

    public void Respawn()
    {
        if (isInRespawnCooldown)
        {
            return;
        }

        if (playerMovement == null)
        {
            playerMovement =
                GetComponent<PlayerMovement>();
        }

        if (playerMovement == null)
        {
            Debug.LogError(
                "PlayerMovementが見つかりません！"
            );

            return;
        }

        if (playerMovement.IsDead)
        {
            return;
        }

        // 直接迷路を再生成せず、
        // まず死亡アニメーションを再生する
        playerMovement.Die();
    }

    // =========================
    // 死亡アニメーション終了後に実行
    // =========================

    public void FinishRespawn()
    {
        if (isInRespawnCooldown)
        {
            return;
        }

        isInRespawnCooldown = true;

        FindMazeGenerator();

        if (mazeGenerator == null)
        {
            Debug.LogError(
                "MazeGeneratorが見つからないため、" +
                "リスポーンできません！"
            );

            isInRespawnCooldown = false;
            return;
        }

        Debug.Log(
            "死亡アニメーション終了！迷路を再生成します"
        );

        ResetRigidbodyVelocity();

        // =========================
        // 迷路の再生成
        // =========================

        // MazeGeneratorがプレイヤーを
        // スタート地点に自動配置する
        mazeGenerator.RestartMaze();

        ResetRigidbodyVelocity();

        // =========================
        // ミニマップのリセット
        // =========================

        if (fogOfWar == null)
        {
            fogOfWar =
                FindFirstObjectByType<FogOfWar>();
        }

        if (fogOfWar != null)
        {
            fogOfWar.ResetExploration();
        }
        else
        {
            Debug.LogWarning(
                "FogOfWarが設定されていません"
            );
        }

        Physics.SyncTransforms();

        Debug.Log(
            "迷路再生成・プレイヤーリスポーン完了！"
        );

        StartCoroutine(RespawnCooldown());
    }

    // =========================
    // Rigidbodyの速度を止める
    // =========================

    private void ResetRigidbodyVelocity()
    {
        if (rb == null || rb.isKinematic)
        {
            return;
        }

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    // =========================
    // 連続死亡防止
    // =========================

    private IEnumerator RespawnCooldown()
    {
        yield return new WaitForSecondsRealtime(
            Mathf.Max(0.1f, respawnCooldown)
        );

        isInRespawnCooldown = false;
    }
}
