
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
    public int smoothResolution = 10;

    [Range(0.05f, 0.4f)]
    public float cornerSmoothness = 0.2f;

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

        List<Vector3> centerPoints =
            new List<Vector3>();

        // 経路を細かく分割して中央位置を計算
        for (int i = 0; i < path.corners.Length - 1; i++)
        {
            Vector3 start = path.corners[i];
            Vector3 end = path.corners[i + 1];

            float distance = Vector3.Distance(start, end);

            int divisions = Mathf.Max(
                2,
                Mathf.CeilToInt(distance / 0.25f)
            );

            for (int j = 0; j < divisions; j++)
            {
                float t = (float)j / divisions;

                Vector3 point = Vector3.Lerp(
                    start,
                    end,
                    t
                );

                centerPoints.Add(GetCenterPosition(
                    point,
                    start,
                    end
                ));
            }
        }

        Vector3 last = path.corners[
            path.corners.Length - 1
        ];

        centerPoints.Add(last);

        // 滑らかに補間
        List<Vector3> smoothPoints =
            SmoothPath(centerPoints);

        routeLine.positionCount = smoothPoints.Count;

        for (int i = 0; i < smoothPoints.Count; i++)
        {
            Vector3 pos = smoothPoints[i];

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

    // 左右の壁の中間位置を取得
    Vector3 GetCenterPosition(
        Vector3 position,
        Vector3 start,
        Vector3 end)
    {
        Vector3 direction = end - start;

        direction.y = 0f;
        direction.Normalize();

        Vector3 right = Vector3.Cross(
            Vector3.up,
            direction
        ).normalized;

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

    // 曲がり角を滑らかにする
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

    IEnumerator HideRoute()
    {
        yield return new WaitForSeconds(displayTime);

        routeLine.enabled = false;
        hideCoroutine = null;
    }
}
