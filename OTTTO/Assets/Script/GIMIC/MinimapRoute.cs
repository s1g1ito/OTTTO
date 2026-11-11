

using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class MinimapRoute : MonoBehaviour
{
    public Transform player;
    public Transform goal;
    public LineRenderer routeLine;

    [Header("Route Settings")]
    public float displayTime = 5f;
    public float heightOffset = 0.15f;
    public float lineWidth = 0.04f;

    [Header("Wall Settings")]
    public LayerMask wallLayer;
    public float rayDistance = 5f;
    public float wallCheckHeight = 0.5f;

    [Header("Path Settings")]
    public int pointsPerSegment = 10;

    [Header("Corner Settings")]
    public float maxCornerOffset = 1.0f;

    private NavMeshPath path;
    private Coroutine hideCoroutine;

    void Start()
    {
        path = new NavMeshPath();

        routeLine.enabled = false;
        routeLine.useWorldSpace = true;

        routeLine.startWidth = lineWidth;
        routeLine.endWidth = lineWidth;

        routeLine.numCornerVertices = 4;
        routeLine.numCapVertices = 4;
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
        if (player == null || goal == null || routeLine == null)
        {
            Debug.LogError("Player、Goal、Route Lineを設定してください");
            return;
        }

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
            Debug.Log("ルートが見つかりません");
            return;
        }

        Vector3[] corners = path.corners;
        int segmentCount = corners.Length - 1;

        Vector3[] centerStart = new Vector3[segmentCount];
        Vector3[] centerEnd = new Vector3[segmentCount];

        // 各通路の中央を計算
        for (int i = 0; i < segmentCount; i++)
        {
            Vector3 start = corners[i];
            Vector3 end = corners[i + 1];

            Vector3 right = GetRightDirection(start, end);

            centerStart[i] = GetCenterPosition(
                Vector3.Lerp(start, end, 0.25f),
                right
            );

            centerEnd[i] = GetCenterPosition(
                Vector3.Lerp(start, end, 0.75f),
                right
            );
        }

        // 曲がり角の中央位置を計算
        Vector3[] turnPoints = new Vector3[corners.Length];

        turnPoints[0] = GetCenterPosition(
            corners[0],
            GetRightDirection(corners[0], corners[1])
        );

        turnPoints[corners.Length - 1] = GetCenterPosition(
            corners[corners.Length - 1],
            GetRightDirection(
                corners[corners.Length - 2],
                corners[corners.Length - 1]
            )
        );

        for (int i = 1; i < corners.Length - 1; i++)
        {
            Vector3 intersection;

            bool success = TryGetIntersection(
                centerStart[i - 1],
                centerEnd[i - 1],
                centerStart[i],
                centerEnd[i],
                out intersection
            );

            // 交点が遠くに飛び出す場合は使用しない
            if (success &&
                Vector3.Distance(intersection, corners[i])
                <= maxCornerOffset)
            {
                intersection.y = corners[i].y;
                turnPoints[i] = intersection;
            }
            else
            {
                // 前後の中央位置から安全な位置を求める
                Vector3 safePoint = (
                    centerEnd[i - 1] +
                    centerStart[i]
                ) * 0.5f;

                safePoint.y = corners[i].y;

                turnPoints[i] = safePoint;
            }
        }

        // ルートの点を作成
        List<Vector3> routePoints = new List<Vector3>();

        for (int i = 0; i < segmentCount; i++)
        {
            Vector3 start = turnPoints[i];
            Vector3 end = turnPoints[i + 1];

            for (int j = 0; j < pointsPerSegment; j++)
            {
                float t = (float)j / pointsPerSegment;

                Vector3 point = Vector3.Lerp(start, end, t);

                routePoints.Add(point);
            }
        }

        routePoints.Add(turnPoints[turnPoints.Length - 1]);

        // LineRendererに設定
        routeLine.positionCount = routePoints.Count;

        for (int i = 0; i < routePoints.Count; i++)
        {
            Vector3 pos = routePoints[i];
            pos.y += heightOffset;

            routeLine.SetPosition(i, pos);
        }

        routeLine.enabled = true;

        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }

        hideCoroutine = StartCoroutine(HideRoute());
    }

    // 通路の左右方向を取得
    Vector3 GetRightDirection(Vector3 start, Vector3 end)
    {
        Vector3 direction = end - start;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            return Vector3.right;

        direction.Normalize();

        return Vector3.Cross(
            Vector3.up,
            direction
        ).normalized;
    }

    // 左右の壁の中央を取得
    Vector3 GetCenterPosition(
        Vector3 position,
        Vector3 right
    )
    {
        Vector3 origin =
            position + Vector3.up * wallCheckHeight;

        RaycastHit leftHit;
        RaycastHit rightHit;

        bool leftFound = Physics.Raycast(
            origin,
            -right,
            out leftHit,
            rayDistance,
            wallLayer,
            QueryTriggerInteraction.Ignore
        );

        bool rightFound = Physics.Raycast(
            origin,
            right,
            out rightHit,
            rayDistance,
            wallLayer,
            QueryTriggerInteraction.Ignore
        );

        if (leftFound && rightFound)
        {
            Vector3 center = (
                leftHit.point + rightHit.point
            ) * 0.5f;

            center.y = position.y;

            return center;
        }

        return position;
    }

    // XZ平面上で交点を計算
    bool TryGetIntersection(
        Vector3 a1,
        Vector3 a2,
        Vector3 b1,
        Vector3 b2,
        out Vector3 intersection
    )
    {
        intersection = Vector3.zero;

        float dx1 = a2.x - a1.x;
        float dz1 = a2.z - a1.z;

        float dx2 = b2.x - b1.x;
        float dz2 = b2.z - b1.z;

        float denominator =
            dx1 * dz2 - dz1 * dx2;

        if (Mathf.Abs(denominator) < 0.001f)
            return false;

        float dx = b1.x - a1.x;
        float dz = b1.z - a1.z;

        float t = (dx * dz2 - dz * dx2) / denominator;

        intersection = new Vector3(
            a1.x + t * dx1,
            a1.y,
            a1.z + t * dz1
        );

        return true;
    }

    // 一定時間後に非表示
    IEnumerator HideRoute()
    {
        yield return new WaitForSeconds(displayTime);

        routeLine.enabled = false;
        hideCoroutine = null;
    }
}










