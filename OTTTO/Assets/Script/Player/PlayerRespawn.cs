
using System.Collections;
using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [Header("リスポーン設定")]
    [SerializeField] private MazeGenerator mazeGenerator;

    [SerializeField] private float fallHeight = -5f;

    [Header("ミニマップ")]
    [SerializeField] private FogOfWar fogOfWar;

    [Header("連続リスポーン防止")]
    [SerializeField] private float respawnCooldown = 0.5f;

    private Rigidbody rb;

    // リスポーン処理中かどうか
    private bool isRespawning = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        FindMazeGenerator();
    }

    private void Update()
    {
        if (isRespawning)
        {
            return;
        }

        // プレイヤーが落下した場合
        if (transform.position.y < fallHeight)
        {
            Respawn();
        }
    }

    // 通常のColliderに接触
    private void OnCollisionEnter(Collision collision)
    {
        if (isRespawning)
        {
            return;
        }

        if (collision.gameObject.CompareTag("Gimick") ||
            collision.gameObject.CompareTag("Enemy"))
        {
            Respawn();
        }
    }

    // TriggerのColliderに接触
    private void OnTriggerEnter(Collider other)
    {
        if (isRespawning)
        {
            return;
        }

        if (other.CompareTag("Gimick") ||
            other.CompareTag("Enemy"))
        {
            Respawn();
        }
    }

    // 現在のステージのMazeGeneratorを取得
    private void FindMazeGenerator()
    {
        if (mazeGenerator == null)
        {
            mazeGenerator =
                FindFirstObjectByType<MazeGenerator>();
        }
    }

    // プレイヤー死亡時の処理
    public void Respawn()
    {
        if (isRespawning)
        {
            return;
        }

        // MazeGeneratorがあるか確認
        FindMazeGenerator();

        if (mazeGenerator == null)
        {
            Debug.LogError(
                "MazeGeneratorが見つからないため、" +
                "リスポーンできません"
            );

            return;
        }

        isRespawning = true;

        Debug.Log("プレイヤーが死亡しました");

        // Rigidbodyの速度を止める
        ResetRigidbodyVelocity();

        // ==============================
        // 迷路の再生成
        // ==============================

        // MazeGenerator側で
        // ① 迷路再生成
        // ② プレイヤーをスタート位置へ配置
        // を実行する
        mazeGenerator.RestartMaze();

        // 移動後の速度を初期化
        ResetRigidbodyVelocity();

        // ==============================
        // ミニマップのリセット
        // ==============================

        if (fogOfWar == null)
        {
            fogOfWar =
                FindFirstObjectByType<FogOfWar>();
        }

        if (fogOfWar != null)
        {
            fogOfWar.ResetExploration();

            Debug.Log(
                "ミニマップの探索履歴を初期化しました"
            );
        }
        else
        {
            Debug.LogWarning(
                "FogOfWarが設定されていません"
            );
        }

        // Colliderの位置を同期
        Physics.SyncTransforms();

        Debug.Log(
            "MazeGeneratorのスタート位置に" +
            "リスポーンしました"
        );

        // 連続死亡を防止
        StartCoroutine(RespawnCooldown());
    }

    // Rigidbodyの速度をリセット
    private void ResetRigidbodyVelocity()
    {
        if (rb == null)
        {
            return;
        }

        if (!rb.isKinematic)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    // リスポーン後のクールダウン
    private IEnumerator RespawnCooldown()
    {
        yield return new WaitForSecondsRealtime(
            Mathf.Max(0.1f, respawnCooldown)
        );

        isRespawning = false;
    }
}
