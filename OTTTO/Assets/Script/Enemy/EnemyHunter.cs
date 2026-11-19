using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyHunter : MonoBehaviour
{
    [Header("徘徊設定")]
    public Transform[] patrolPoints;
    public float arriveDistance = 0.5f;

    [Header("移動設定")]
    public float moveSpeed = 3f;

    [Header("Player発見設定")]
    public float viewDistance = 10f;

    // 扇形全体の角度
    public float viewAngle = 90f;

    public float rayHeight = 1f;

    [Header("追跡設定")]
    public float loseDistance = 20f;

    [Header("視界を遮るレイヤー")]
    [Tooltip("Wallなど、視界を遮るLayerだけを設定してください。Groundは入れません。")]
    public LayerMask visionObstacleMask;

    private NavMeshAgent agent;
    private Transform player;

    private int currentPoint = 0;

    // 追跡中か
    private bool isChasing = false;

    void Start()
    {
        // ========================================
        // NavMeshAgent
        // ========================================

        agent =
            GetComponent<NavMeshAgent>();

        agent.speed =
            moveSpeed;

        agent.stoppingDistance =
            arriveDistance;

        // ========================================
        // Player取得
        // ========================================

        GameObject obj =
            GameObject.FindGameObjectWithTag("Player");

        if (obj != null)
        {
            player =
                obj.transform;

            Debug.Log(
                "EnemyHunter：Playerを取得しました"
            );
        }
        else
        {
            Debug.LogError(
                "EnemyHunter：Playerタグのオブジェクトが見つかりません！"
            );
        }

        // ========================================
        // NavMesh確認
        // ========================================

        if (!agent.isOnNavMesh)
        {
            Debug.LogError(
                "EnemyHunter：NavMesh上にいません！"
            );

            return;
        }

        // ========================================
        // 最初の徘徊地点
        // ========================================

        if (patrolPoints != null &&
            patrolPoints.Length > 0)
        {
            agent.SetDestination(
                patrolPoints[0].position
            );
        }

        // ========================================
        // ここでは追跡開始しない
        // ========================================

        isChasing = false;
    }

    void Update()
    {
        if (player == null ||
            agent == null ||
            !agent.enabled)
        {
            return;
        }

        if (!agent.isOnNavMesh)
        {
            return;
        }

        // ========================================
        // 追跡中
        // ========================================

        if (isChasing)
        {
            float distance =
                Vector3.Distance(
                    transform.position,
                    player.position
                );

            // Playerが遠くなりすぎた
            if (distance > loseDistance)
            {
                StopChase();

                return;
            }

            agent.isStopped =
                false;

            // Playerを追跡
            agent.SetDestination(
                player.position
            );

            return;
        }

        // ========================================
        // 追跡していない
        // ========================================

        if (CanSeePlayer())
        {
            Debug.Log(
                "★★ 敵Bが自分でPlayerを発見！ ★★"
            );

            StartChase();

            return;
        }

        // ========================================
        // 徘徊
        // ========================================

        Patrol();
    }

    // ========================================
    // 徘徊
    // ========================================

    void Patrol()
    {
        if (patrolPoints == null ||
            patrolPoints.Length == 0)
        {
            return;
        }

        if (patrolPoints[currentPoint] == null)
        {
            return;
        }

        if (!agent.pathPending &&
            agent.remainingDistance <= arriveDistance)
        {
            currentPoint =
                (currentPoint + 1) %
                patrolPoints.Length;

            agent.SetDestination(
                patrolPoints[currentPoint].position
            );
        }
    }

    // ========================================
    // 敵B自身のPlayer視認
    // ========================================

    bool CanSeePlayer()
    {
        if (player == null)
        {
            return false;
        }

        // ========================================
        // 目の位置
        // ========================================

        Vector3 origin =
            transform.position +
            Vector3.up * rayHeight;

        // Playerの胸
        Vector3 target =
            player.position +
            Vector3.up * 1.0f;

        Vector3 direction =
            target - origin;

        // ========================================
        // 水平方向
        // ========================================

        Vector3 flatDirection =
            new Vector3(
                direction.x,
                0f,
                direction.z
            );

        float distance =
            flatDirection.magnitude;

        // ========================================
        // 距離
        // ========================================

        if (distance > viewDistance)
        {
            return false;
        }

        // ========================================
        // 角度
        // ========================================

        float angle =
            Vector3.Angle(
                transform.forward,
                flatDirection
            );

        if (angle > viewAngle / 2f)
        {
            return false;
        }

        // ========================================
        // 遮蔽物
        // ========================================

        Vector3 rayDirection =
            direction.normalized;

        float rayDistance =
            direction.magnitude;

        if (Physics.Raycast(
            origin,
            rayDirection,
            out RaycastHit hit,
            rayDistance,
            visionObstacleMask))
        {
            return false;
        }

        // ========================================
        // Player発見
        // ========================================

        return true;
    }

    // ========================================
    // 追跡開始
    // ========================================

    public void StartChase()
    {
        if (isChasing)
        {
            return;
        }

        Debug.Log(
            "敵B：追跡通知を受信！"
        );

        if (agent == null ||
            !agent.enabled)
        {
            Debug.LogError(
                "敵B：NavMeshAgentが無効です！"
            );

            return;
        }

        if (!agent.isOnNavMesh)
        {
            Debug.LogError(
                "敵B：NavMesh上にいません！"
            );

            return;
        }

        if (player == null)
        {
            Debug.LogError(
                "敵B：Playerが取得できていません！"
            );

            return;
        }

        isChasing =
            true;

        agent.isStopped =
            false;

        agent.SetDestination(
            player.position
        );

        Debug.Log(
            "敵B：追跡開始！目的地：" +
            player.position
        );
    }

    // ========================================
    // 追跡終了
    // ========================================

    void StopChase()
    {
        isChasing =
            false;

        if (agent != null &&
            agent.isOnNavMesh)
        {
            agent.ResetPath();
        }

        Debug.Log(
            "敵B：追跡終了、徘徊再開！"
        );
    }
}