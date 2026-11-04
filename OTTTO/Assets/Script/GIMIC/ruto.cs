
using UnityEngine;

public class roto : MonoBehaviour
{
    public Transform player;
    public Transform goal;
    public LineRenderer routeLine;

    private bool showRoute = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            showRoute = !showRoute;
            routeLine.enabled = showRoute;
        }

        if (showRoute)
        {
            routeLine.positionCount = 2;

            routeLine.SetPosition(
                0, player.position + Vector3.up * 2f
            );

            routeLine.SetPosition(
                1, goal.position + Vector3.up * 2f
            );
        }
    }
}
