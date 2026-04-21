using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyFollow : MonoBehaviour
{
    // 追いかける対象のゲームオブジェクト
    [SerializeField]
    private GameObject player;

    // プレイヤーを追いかける距離のしきい値
    [SerializeField] private float chaseDistance = 10f;

    // 周回用のルートを設定する
    [SerializeField] private Transform[] m_markers = null;
    private int currentMarkerIndex = 0;
    // 巡回ポイントに到達したかの判定用
    [SerializeField] private float patrolArriveThreshold = 0.5f;

    // NavMeshAgentコンポーネントを入れる
    private NavMeshAgent navMesAgent;

    // Start is called before the first frame update
    void Start()
    {
        // 自分自身のNavMeshAgentを入れる
        navMesAgent = this.gameObject.GetComponent<NavMeshAgent>();

        // 最初のマーカーをセット
        if (m_markers != null && m_markers.Length > 0)
        {
            navMesAgent.destination = m_markers[0].position;
        }
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);

        if (distance < chaseDistance)
        {
            // プレイヤーが近くにいる場合だけ追いかける
            navMesAgent.destination = player.transform.position;
        }
        else
        {
            // プレイヤーがいないときは巡回ルートを移動
            Patrol();
        }

        void Patrol()
        {
            if (m_markers == null || m_markers.Length == 0) return;

            // NavMeshAgentの移動が終わったか確認（pathPendingは移動中の判定）
            if (!navMesAgent.pathPending && navMesAgent.remainingDistance <= patrolArriveThreshold)
            {
                // 次のマーカーへ
                currentMarkerIndex = (currentMarkerIndex + 1) % m_markers.Length;
                navMesAgent.destination = m_markers[currentMarkerIndex].position;
            }
        }
    }
}