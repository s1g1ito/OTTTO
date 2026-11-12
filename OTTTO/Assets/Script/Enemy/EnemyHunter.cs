using UnityEngine;
using UnityEngine.AI;

public class EnemyHunter : MonoBehaviour
{
    [Header("徘徊設定")]
    public Transform[] patrolPoints;
    public float arriveDistance = 0.5f;

    [Header("Player発見設定")]
    public float viewDistance = 10f;
    public float viewAngle = 90f;
    public float rayHeight = 1f;

    [Header("追跡設定")]
    public float loseDistance = 20f;

    private NavMeshAgent agent;
    private Transform player;

    private int currentPoint = 0;
    private bool isChasing = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        GameObject obj =
            GameObject.FindGameObjectWithTag("Player");

        if (obj != null)
            player = obj.transform;

        // 最初は徘徊
        if (patrolPoints != null &&
            patrolPoints.Length > 0)
        {
            agent.SetDestination(
                patrolPoints[0].position);
        }

        Invoke("StartChase", 3f);
    }

    void Update()
    {
        if (player == null || agent == null)
            return;

        if (isChasing)
        {
            float distance = Vector3.Distance(
                transform.position,
                player.position
            );

            if (distance > loseDistance)
            {
                StopChase();
                return;
            }

            agent.isStopped = false;
            agent.SetDestination(player.position);
        }
        else
        {
            if (CanSeePlayer())
            {
                StartChase();
                return;
            }

            Patrol();
        }
    }

    // 徘徊処理
    void Patrol()
    {
        if (patrolPoints == null ||
            patrolPoints.Length == 0)
            return;

        if (!agent.pathPending &&
            agent.remainingDistance <= arriveDistance)
        {
            currentPoint =
                (currentPoint + 1) % patrolPoints.Length;

            agent.SetDestination(
                patrolPoints[currentPoint].position);
        }
    }

    // Playerの視界判定
    bool CanSeePlayer()
    {
        Vector3 origin =
            transform.position + Vector3.up * rayHeight;

        Vector3 direction =
            player.position - origin;

        Vector3 flatDirection =
            new Vector3(direction.x, 0, direction.z);

        // 距離判定
        if (flatDirection.magnitude > viewDistance)
            return false;

        // 角度判定
        if (Vector3.Angle(
            transform.forward,
            flatDirection) > viewAngle)
            return false;

        // 遮蔽物判定
        if (Physics.Raycast(
            origin,
            direction.normalized,
            out RaycastHit hit,
            viewDistance))
        {
            return hit.collider.CompareTag("Player");
        }

        return false;
    }

    // 追跡開始
    public void StartChase()
    {
        Debug.Log("敵B：追跡通知を受信！");

        if (agent == null || !agent.enabled)
        {
            Debug.LogError("NavMeshAgentが無効です！");
            return;
        }

        if (!agent.isOnNavMesh)
        {
            Debug.LogError("敵BがNavMesh上にいません！");
            return;
        }

        if (player == null)
        {
            Debug.LogError("Playerが取得できていません！");
            return;
        }

        isChasing = true;

        agent.isStopped = false;
        agent.SetDestination(player.position);

        Debug.Log("敵B：追跡開始！目的地：" + player.position);
    }

    // 追跡終了
    void StopChase()
    {
        isChasing = false;

        // 現在の経路を解除
        agent.ResetPath();

        Debug.Log("EnemyHunter：追跡終了、徘徊再開！");
    }
}
