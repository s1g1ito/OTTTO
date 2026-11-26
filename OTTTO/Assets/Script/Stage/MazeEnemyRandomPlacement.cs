
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

/// <summary>
/// MazeNavMeshAutoBuilderがNavMeshを構築したあと、
/// 敵をランダムな通路に配置し、EnemyHunterの巡回地点を再設定する。
/// Stage1/2/3共通で使用できる。
/// </summary>
[DisallowMultipleComponent]
[DefaultExecutionOrder(1100)]
public class MazeEnemyRandomPlacement : MonoBehaviour
{
    [Header("迷路・NavMesh")]
    [SerializeField] private MazeGenerator mazeGenerator;
    [SerializeField] private Transform mazeRoot;
    [SerializeField] private NavMeshSurface navMeshSurface;

    [Header("配置する敵（NavMeshAgent）")]
    [SerializeField] private NavMeshAgent[] enemies;
    [SerializeField] private Transform player;

    [Header("敵の配置設定")]
    [Min(0f)]
    [SerializeField] private float minDistanceFromPlayer = 15f;
    [Min(0f)]
    [SerializeField] private float minDistanceBetweenEnemies = 10f;

    [Header("巡回地点の自動配置")]
    [Min(0)]
    [SerializeField] private int patrolPointCount = 5;
    [Min(0f)]
    [SerializeField] private float minPatrolPointDistance = 5f;

    [Header("NavMesh上の位置を探す設定")]
    [Min(0.1f)]
    [SerializeField] private float cellScale = 5f;
    [SerializeField] private float searchStartHeight = 1.5f;
    [Min(0.1f)]
    [SerializeField] private float navMeshSearchRadius = 2.5f;
    [Min(1)]
    [SerializeField] private int maxSearchAttempts = 200;

    private int lastPlacedMazeCellId;
    private int lastPlacedNavMeshDataId;
    private Transform generatedPatrolRoot;
    private bool warnedMissingSettings;

    private void Awake()
    {
        FindReferences();
    }

    private void OnEnable()
    {
        lastPlacedMazeCellId = 0;
        lastPlacedNavMeshDataId = 0;
    }

    private void LateUpdate()
    {
        if (!CheckSettings()) return;

        // 生成された迷路のマスを確認する
        int currentCellId = 0;
        int activeCellCount = 0;

        foreach (Transform cell in mazeRoot)
        {
            if (!cell.gameObject.activeInHierarchy ||
                !cell.TryGetComponent<MazeCell>(out _))
            {
                continue;
            }

            activeCellCount++;

            if (currentCellId == 0)
            {
                currentCellId = cell.GetInstanceID();
            }
        }

        // 迷路やNavMeshが未完成なら待つ
        if (currentCellId == 0 ||
            activeCellCount != mazeGenerator.width * mazeGenerator.height ||
            navMeshSurface.navMeshData == null)
        {
            return;
        }

        int currentNavMeshId =
            navMeshSurface.navMeshData.GetInstanceID();

        // すでに配置済みの迷路なら何もしない
        if (currentCellId == lastPlacedMazeCellId &&
            currentNavMeshId == lastPlacedNavMeshDataId)
        {
            return;
        }

        // 同じ迷路で繰り返し配置しない
        lastPlacedMazeCellId = currentCellId;
        lastPlacedNavMeshDataId = currentNavMeshId;

        PlaceEnemiesAndPatrolPoints();
    }

    private void FindReferences()
    {
        if (mazeGenerator == null)
        {
            mazeGenerator = GetComponent<MazeGenerator>();
        }

        if (navMeshSurface == null && mazeRoot != null)
        {
            navMeshSurface =
                mazeRoot.GetComponent<NavMeshSurface>();
        }

        if (player == null)
        {
            GameObject playerObject =
                GameObject.FindGameObjectWithTag("Player");

            if (playerObject != null)
            {
                player = playerObject.transform;
            }
        }
    }

    private bool CheckSettings()
    {
        FindReferences();

        bool valid =
            mazeGenerator != null &&
            mazeRoot != null &&
            navMeshSurface != null &&
            navMeshSurface.isActiveAndEnabled &&
            enemies != null &&
            enemies.Length > 0 &&
            cellScale > 0f;

        if (!valid && !warnedMissingSettings)
        {
            Debug.LogError(
                "MazeEnemyRandomPlacement: " +
                "迷路、NavMeshSurface、敵の設定を確認してください",
                this
            );

            warnedMissingSettings = true;
        }

        if (valid)
        {
            warnedMissingSettings = false;
        }

        return valid;
    }

    private void PlaceEnemiesAndPatrolPoints()
    {
        // 古い巡回地点を削除
        if (generatedPatrolRoot != null)
        {
            generatedPatrolRoot.gameObject.SetActive(false);
            Destroy(generatedPatrolRoot.gameObject);
        }

        generatedPatrolRoot =
            new GameObject("GeneratedPatrolPoints").transform;

        generatedPatrolRoot.SetParent(transform, false);

        List<Vector3> enemyPositions =
            new List<Vector3>();

        // 敵を順番にランダム配置
        foreach (NavMeshAgent agent in enemies)
        {
            if (agent == null ||
                !agent.enabled ||
                !agent.gameObject.activeInHierarchy)
            {
                Debug.LogWarning(
                    "配置対象の敵が未設定または無効です",
                    this
                );

                continue;
            }

            // プレイヤーや他の敵から離れた通路を探す
            if (!TryFindWalkablePosition(
                agent,
                enemyPositions,
                minDistanceBetweenEnemies,
                player,
                minDistanceFromPlayer,
                out Vector3 spawnPosition))
            {
                Debug.LogWarning(
                    agent.name + " の配置場所を見つけられませんでした",
                    agent
                );

                continue;
            }

            if (agent.isOnNavMesh)
            {
                agent.ResetPath();
            }

            // 敵をNavMesh上へ移動
            bool warped = agent.Warp(spawnPosition);

            if (!warped)
            {
                // AgentがNavMeshに未接続の場合の復帰処理
                agent.enabled = false;
                agent.transform.position = spawnPosition;
                agent.enabled = true;
            }

            if (!agent.isOnNavMesh)
            {
                Debug.LogError(
                    agent.name + " をNavMeshに配置できませんでした",
                    agent
                );

                continue;
            }

            enemyPositions.Add(agent.transform.position);
            agent.ResetPath();

            // Rigidbodyとの競合を確認
            Rigidbody rb = agent.GetComponent<Rigidbody>();

            if (rb != null && !rb.isKinematic)
            {
                Debug.LogWarning(
                    agent.name +
                    " のRigidbodyが非Kinematicです。" +
                    "NavMeshAgentとの競合に注意してください",
                    agent
                );
            }

            // 既存のEnemyHunterを取得
            EnemyHunter hunter =
                agent.GetComponent<EnemyHunter>();

            if (hunter == null)
            {
                continue;
            }

            List<Transform> points =
                new List<Transform>();

            NavMeshPath path = new NavMeshPath();

            // この敵が到達可能な巡回地点を生成
            for (int i = 0; i < patrolPointCount; i++)
            {
                bool found = false;

                for (int attempt = 0;
                     attempt < maxSearchAttempts;
                     attempt++)
                {
                    if (!TryFindWalkablePosition(
                        agent,
                        null,
                        0f,
                        agent.transform,
                        minPatrolPointDistance,
                        out Vector3 candidate,
                        1))
                    {
                        continue;
                    }

                    // 巡回地点同士の重なりを避ける
                    bool tooClose = false;

                    foreach (Transform point in points)
                    {
                        if (FlatDistance(
                            point.position,
                            candidate) < minPatrolPointDistance)
                        {
                            tooClose = true;
                            break;
                        }
                    }

                    if (tooClose)
                    {
                        continue;
                    }

                    // 実際に移動可能な経路か確認
                    if (!agent.CalculatePath(candidate, path) ||
                        path.status != NavMeshPathStatus.PathComplete)
                    {
                        continue;
                    }

                    GameObject marker = new GameObject(
                        agent.name + "_Patrol_" + (i + 1)
                    );

                    marker.transform.SetParent(
                        generatedPatrolRoot,
                        false
                    );

                    marker.transform.position = candidate;

                    points.Add(marker.transform);

                    found = true;
                    break;
                }

                if (!found)
                {
                    Debug.LogWarning(
                        agent.name +
                        " の巡回地点を一部作成できませんでした",
                        agent
                    );

                    break;
                }
            }

            // 固定Markerを今回のランダムMarkerに差し替える
            hunter.patrolPoints = points.ToArray();

            // 最初の巡回地点へ移動
            if (points.Count > 0)
            {
                agent.isStopped = false;
                agent.SetDestination(points[0].position);
            }

            Debug.Log(
                agent.name +
                " を通路に配置しました。巡回地点: " +
                points.Count + "個",
                agent
            );
        }
    }

    // ランダムなマスからNavMesh上の位置を探す
    private bool TryFindWalkablePosition(
        NavMeshAgent agent,
        List<Vector3> otherEnemies,
        float otherEnemyDistance,
        Transform avoidTarget,
        float avoidDistance,
        out Vector3 result,
        int attemptCount = -1)
    {
        result = Vector3.zero;

        // 敵のAgent Typeに合ったNavMeshを検索
        NavMeshQueryFilter filter =
            new NavMeshQueryFilter
            {
                agentTypeID = agent.agentTypeID,
                areaMask = agent.areaMask
            };

        if (attemptCount < 0)
        {
            attemptCount = maxSearchAttempts;
        }

        for (int attempt = 0;
             attempt < attemptCount;
             attempt++)
        {
            int x = Random.Range(0, mazeGenerator.width);
            int y = Random.Range(0, mazeGenerator.height);

            // スタートとゴールのマスを避ける
            if ((x == 0 && y == 0) ||
                (x == mazeGenerator.width - 1 &&
                 y == mazeGenerator.height - 1))
            {
                continue;
            }

            Vector3 cellCenter = new Vector3(
                x * cellScale,
                searchStartHeight,
                y * cellScale
            );

            // NavMesh上の位置を取得
            if (!NavMesh.SamplePosition(
                cellCenter,
                out NavMeshHit hit,
                navMeshSearchRadius,
                filter))
            {
                continue;
            }

            // マス中心から遠すぎる場所は除外
            if (FlatDistance(
                cellCenter,
                hit.position) > navMeshSearchRadius)
            {
                continue;
            }

            // プレイヤーや敵の近くを避ける
            if (avoidTarget != null &&
                FlatDistance(
                    hit.position,
                    avoidTarget.position) < avoidDistance)
            {
                continue;
            }

            bool nearOtherEnemy = false;

            if (otherEnemies != null)
            {
                foreach (Vector3 otherPosition in otherEnemies)
                {
                    if (FlatDistance(
                        hit.position,
                        otherPosition) < otherEnemyDistance)
                    {
                        nearOtherEnemy = true;
                        break;
                    }
                }
            }

            if (nearOtherEnemy)
            {
                continue;
            }

            result = hit.position;
            return true;
        }

        return false;
    }

    private static float FlatDistance(
        Vector3 a,
        Vector3 b)
    {
        return Vector2.Distance(
            new Vector2(a.x, a.z),
            new Vector2(b.x, b.z)
        );
    }
}
