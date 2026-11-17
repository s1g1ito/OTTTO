
using UnityEngine;

public class RouteButton : MonoBehaviour
{
    [Header("Route Settings")]
    public MinimapRoute minimapRoute;

    [Header("Player Settings")]
    public Transform player;

    private BoxCollider boxCollider;
    private Collider[] playerColliders;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider>();

        if (boxCollider == null)
        {
            Debug.LogError("Box Colliderがありません");
            return;
        }

        if (player != null)
        {
            playerColliders = player.GetComponentsInChildren<Collider>();
        }
    }

    // ボタンをクリックしたとき
    void OnMouseDown()
    {
        Debug.Log("ボタンがクリックされました");

        if (player == null || minimapRoute == null ||
            boxCollider == null)
        {
            Debug.LogError("Inspectorの設定を確認してください");
            return;
        }

        // プレイヤーとBox Colliderが重なっているか確認
        if (IsPlayerInside())
        {
            Debug.Log("範囲内！ルートを表示します");

            minimapRoute.ShowRoute();
        }
        else
        {
            Debug.Log("範囲外です");
        }
    }

    // プレイヤーのColliderとボタンのColliderを比較
    bool IsPlayerInside()
    {
        if (playerColliders == null)
            return false;

        foreach (Collider playerCol in playerColliders)
        {
            if (playerCol == null || !playerCol.enabled)
                continue;

            Vector3 direction;
            float distance;

            bool overlap = Physics.ComputePenetration(
                boxCollider,
                transform.position,
                transform.rotation,
                playerCol,
                playerCol.transform.position,
                playerCol.transform.rotation,
                out direction,
                out distance
            );

            if (overlap)
                return true;
        }

        return false;
    }
}





