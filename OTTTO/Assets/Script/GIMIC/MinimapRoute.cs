
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
    public float lineWidth = 0.06f;

    [Header("Wall Settings")]
    public LayerMask wallLayer;
    public float rayDistance = 5f;
    public float wallCheckHeight = 0.5f;

    [Header("Smooth Settings")]
    [Range(2, 20)]
    public int smoothResolution = 8;

    [Range(0.05f, 0.4f)]
    public float cornerSmoothness = 0.05f;

    private NavMeshPath path;
    private Coroutine hideCoroutine;

    void Start()
    {
        path = new NavMeshPath();

        routeLine.enabled = false;
        routeLine.useWorldSpace = true;

        routeLine.startWidth = lineWidth;
        routeLine.endWidth = lineWidth;

        routeLine.numCornerVertices = 8;
        routeLine.numCapVertices = 8;
    }

    void Update()
    {
        // Mキーでルート表示
        if (Input.GetKeyDown(KeyCode.M))
        {
            ShowRoute();
        }
    }

    void ShowRoute()
    {
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

        List<Vector3> routePoints = new List<Vector3>();

        // NavMeshの経路を細かく分割
        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            Vector3 start = path.corners[i];
            Vector3 end = path.corners[i + 1];

            float distance = Vector3.Distance(start, end);

            int divisions = Mathf.Max(
                2,
                Mathf.CeilToInt(distance / 0.2f)
            );

            Vector3 direction = (end - start).normalized;

            direction.y = 0;
            direction.Normalize();

            Vector3 right = Vector3.Cross(
                Vector3.up,
                direction
            ).normalized;

            for (int j = 0; j < divisions; j++)
            {
                float t = (float)j / divisions;

                Vector3 point = Vector3.Lerp(
                    start,
                    end,
                    t
                );

                // 通路の中央位置を取得
                Vector3 center = GetCenterPosition(
                    point,
                    right
                );

                routePoints.Add(center);
            }
        }

        // ゴール地点も中央に補正
        Vector3 lastStart =
            path.corners[path.corners.Length - 2];

        Vector3 lastEnd =
            path.corners[path.corners.Length - 1];

        Vector3 lastDirection =
            (lastEnd - lastStart).normalized;

        lastDirection.y = 0;
        lastDirection.Normalize();

        Vector3 lastRight = Vector3.Cross(
            Vector3.up,
            lastDirection
        ).normalized;

        routePoints.Add(
            GetCenterPosition(lastEnd, lastRight)
        );

        // ルートを滑らかにする
        List<Vector3> smoothPoints =
            SmoothPath(routePoints);

        routeLine.positionCount = smoothPoints.Count;

        for (int i = 0; i < smoothPoints.Count; i++)
        {
            Vector3 pos = smoothPoints[i];

            pos.y += heightOffset;

            routeLine.SetPosition(i, pos);
        }

        routeLine.enabled = true;

        // 前の非表示処理を停止
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }

        hideCoroutine = StartCoroutine(HideRoute());
    }

    // 左右の壁から通路の中央を取得
    Vector3 GetCenterPosition(
        Vector3 position,
        Vector3 right)
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

        // 左右の壁が両方見つかった場合
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

    // ルートの角を滑らかにする
    List<Vector3> SmoothPath(List<Vector3> points)
    {
        if (points.Count < 3)
            return points;

        List<Vector3> result =
            new List<Vector3>();

        result.Add(points[0]);

        for (int i = 1; i < points.Count - 1; i++)
        {
            Vector3 previous = points[i - 1];
            Vector3 current = points[i];
            Vector3 next = points[i + 1];

            Vector3 pointA = Vector3.Lerp(
                current,
                previous,
                cornerSmoothness
            );

            Vector3 pointB = Vector3.Lerp(
                current,
                next,
                cornerSmoothness
            );

            result.Add(pointA);

            for (int j = 1; j < smoothResolution; j++)
            {
                float t = (float)j / smoothResolution;

                Vector3 p = Vector3.Lerp(
                    Vector3.Lerp(pointA, current, t),
                    Vector3.Lerp(current, pointB, t),
                    t
                );

                result.Add(p);
            }

            result.Add(pointB);
        }

        result.Add(points[points.Count - 1]);

        return result;
    }

    // 一定時間後にルートを非表示
    IEnumerator HideRoute()
    {
        yield return new WaitForSeconds(displayTime);

        routeLine.enabled = false;
        hideCoroutine = null;
    }
}

