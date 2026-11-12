
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class MinimapGuide : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform goal;
    [SerializeField] private LineRenderer routeLine;
    [SerializeField] private float displayTime = 3f;

    private Coroutine hideCoroutine;

    void Update()
    {
        // MinimapRouteがルート表示を担当する
    }

    void ShowRoute()
    {
        // 未設定チェック
        if (player == null)
        {
            Debug.LogError("MinimapGuide: Playerが設定されていません");
            return;
        }

        if (goal == null)
        {
            Debug.LogError("MinimapGuide: Goalが設定されていません");
            return;
        }

        if (routeLine == null)
        {
            Debug.LogError("MinimapGuide: Route Lineが設定されていません");
            return;
        }

        NavMeshPath path = new NavMeshPath();

        bool found = NavMesh.CalculatePath(
            player.position,
            goal.position,
            NavMesh.AllAreas,
            path
        );

        if (!found ||
            path.status != NavMeshPathStatus.PathComplete ||
            path.corners.Length < 2)
        {
            Debug.LogWarning("ルートが見つかりません");
            return;
        }

        routeLine.positionCount = path.corners.Length;

        for (int i = 0; i < path.corners.Length; i++)
        {
            Vector3 pos = path.corners[i];

            // ミニマップ用の高さ
            pos.y = 0.5f;

            routeLine.SetPosition(i, pos);
        }

        routeLine.gameObject.SetActive(true);

        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }

        hideCoroutine = StartCoroutine(
            HideRouteAfterSeconds()
        );
    }

    IEnumerator HideRouteAfterSeconds()
    {
        yield return new WaitForSeconds(displayTime);

        if (routeLine != null)
        {
            routeLine.gameObject.SetActive(false);
        }

        hideCoroutine = null;
    }
}
