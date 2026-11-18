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

    // 追跡中か
    private bool isChasing = false;

    void Start()
    {
        agent =
            GetComponent<NavMeshAgent>();

        // Player取得
        GameObject obj =
            GameObject.FindGameObjectWithTag("Player");

        if (obj != null)
        {
            player =
                obj.transform;
        }
        else
        {
            Debug.LogError(
                "EnemyHunter：Playerタグのオブジェクトが見つかりません！"
            );
        }

        // 最初は徘徊
        if (patrolPoints != null &&
            patrolPoints.Length > 0)
        {
            agent.SetDestination(
                patrolPoints[0].position
            );
        }

        // ここでInvokeしない
        // 敵Aから通知されるか、
        // 敵B自身がPlayerを発見したら追跡する
    }

    void Update()
    {
        if (player == null ||
            agent == null)
        {
            return;
        }


        // 追跡中


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


        // 追跡していない


        // 敵B自身がPlayerを発見
        if (CanSeePlayer())
        {
            Debug.Log(
                "★★ 敵Bが自分でPlayerを発見！ ★★"
            );

            StartChase();

            return;
        }

        // Playerを見つけていないので徘徊
        Patrol();
    }


    // 徘徊


    void Patrol()
    {
        if (patrolPoints == null ||
            patrolPoints.Length == 0)
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


    // 敵B自身のPlayer視認判定


    bool CanSeePlayer()
    {
        // 敵Bの目の位置
        Vector3 origin =
            transform.position +
            Vector3.up * rayHeight;

        // Playerの胸あたり
        Vector3 target =
            player.position +
            Vector3.up * 1.0f;

        Vector3 direction =
            target - origin;

        // 水平方向
        Vector3 flatDirection =
            new Vector3(
                direction.x,
                0f,
                direction.z
            );

        float distance =
            flatDirection.magnitude;

 
        //距離
    

        if (distance > viewDistance)
        {
            return false;
        }

        
        //角度
        

        float angle =
            Vector3.Angle(
                transform.forward,
                flatDirection
            );

        if (angle > viewAngle)
        {
            return false;
        }

        
        //遮蔽物
        

        if (Physics.Raycast(
            origin,
            direction.normalized,
            out RaycastHit hit,
            direction.magnitude))
        {
            // Player本体
            if (hit.transform == player)
            {
                return true;
            }

            // Playerの子オブジェクト
            if (hit.transform.IsChildOf(player))
            {
                return true;
            }

            // 壁などに当たった
            return false;
        }

        return false;
    }

   
    //追跡開始
    

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

   
    // 追跡終了
    

    void StopChase()
    {
        isChasing =
            false;

        agent.ResetPath();

        Debug.Log(
            "敵B：追跡終了、徘徊再開！"
        );
    }
}