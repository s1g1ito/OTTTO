using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class MinimapRoute : MonoBehaviour
{
    [Header("Target")]
    public Transform player;
    public Transform goal;

    [Header("Route")]
    public LineRenderer routeLine;
    public float displayTime = 5f;
    public float lineHeight = 0.5f;

    private Coroutine hideCoroutine;
    private NavMeshPath path;

    void Start()
    {
        routeLine.enabled = false;
        path = new NavMeshPath();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            ShowRoute();
        }
    }

    void ShowRoute()
    {
        // プレイヤーとゴールがNavMesh上にあるか確認
        NavMeshHit playerHit;
        NavMeshHit goalHit;

        if (!NavMesh.SamplePosition(
            player.position, out playerHit, 2f,
            NavMesh.AllAreas))
        {
            Debug.LogWarning("PlayerがNavMesh上にありません");
            return;
        }

        if (!NavMesh.SamplePosition(
            goal.position, out goalHit, 2f,
            NavMesh.AllAreas))
        {
            Debug.LogWarning("GoalがNavMesh上にありません");
            return;
        }

        // 壁を避ける経路を計算
        bool found = NavMesh.CalculatePath(
            playerHit.position,
            goalHit.position,
            NavMesh.AllAreas,
            path
        );

        if (!found || path.status != NavMeshPathStatus.PathComplete)
        {
            Debug.LogWarning("ゴールまでの経路が見つかりません");
            return;
        }

        // 曲がり角を含む経路を表示
        Vector3[] corners = path.corners;

        routeLine.positionCount = corners.Length;

        for (int i = 0; i < corners.Length; i++)
        {
            routeLine.SetPosition(
                i,
                corners[i] + Vector3.up * lineHeight
            );
        }

        routeLine.enabled = true;

        // 前回の非表示処理を停止
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }

        hideCoroutine = StartCoroutine(HideRoute());
    }

    IEnumerator HideRoute()
    {
        yield return new WaitForSeconds(displayTime);

        routeLine.enabled = false;
        hideCoroutine = null;
    }
}
