
using UnityEngine;

public class RouteButton : MonoBehaviour
{
    public MinimapRoute minimapRoute;

    void OnMouseDown()
    {
        if (minimapRoute != null)
        {
            minimapRoute.ShowRoute();

            Debug.Log("ルート表示ボタンが押されました");
        }
        else
        {
            Debug.LogError("MinimapRouteが設定されていません");
        }
    }
}


