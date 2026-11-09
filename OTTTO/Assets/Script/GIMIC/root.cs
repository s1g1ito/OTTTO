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
        if (Input.GetKeyDown(KeyCode.M))
        {
            ShowRoute();
        }
    }

    void ShowRoute()
    {
        NavMeshPath path = new NavMeshPath();

        if (!NavMesh.CalculatePath(
                player.position,
                goal.position,
                NavMesh.AllAreas,
                path))
        {
            return;
        }

        routeLine.positionCount = path.corners.Length;

        for (int i = 0; i < path.corners.Length; i++)
        {
            // ミニマップ用の高さに固定
            Vector3 pos = path.corners[i];
            pos.y = 0.5f;

            routeLine.SetPosition(i, pos);
        }

        routeLine.gameObject.SetActive(true);

        if (hideCoroutine != null)
            StopCoroutine(hideCoroutine);

        hideCoroutine = StartCoroutine(HideRouteAfterSeconds());
    }

    IEnumerator HideRouteAfterSeconds()
    {
        yield return new WaitForSeconds(displayTime);

        routeLine.gameObject.SetActive(false);
    }
}
