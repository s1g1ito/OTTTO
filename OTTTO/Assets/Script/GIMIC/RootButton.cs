
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
        // ボタンについているBox Colliderを取得
        boxCollider = GetComponent<BoxCollider>();

        if (boxCollider == null)
        {
            Debug.LogError("Box Colliderがありません");
            return;
        }

        // PlayerのColliderを取得
        if (player != null)
        {
            playerColliders =
                player.GetComponentsInChildren<Collider>();
        }
        else
        {
            Debug.LogError("Playerが設定されていません");
        }
    }

    // ボタンをクリックしたとき
    void OnMouseDown()
    {
        Debug.Log("① ボタンがクリックされました");

        if (player == null ||
            minimapRoute == null ||
            boxCollider == null)
        {
            Debug.LogError("Inspectorの設定を確認してください");
            return;
        }

        // プレイヤーが範囲内にいるか確認
        if (IsPlayerInside())
        {
            Debug.Log("② 範囲内！ルートを表示します");

            minimapRoute.ShowRoute();
        }
        else
        {
            Debug.Log("範囲外です");
        }
    }

    // プレイヤーとBox Colliderの重なりを確認
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
                boxCollider.transform.position,
                boxCollider.transform.rotation,

                playerCol,
                playerCol.transform.position,
                playerCol.transform.rotation,

                out direction,
                out distance
            );

            if (overlap)
            {
                return true;
            }
        }

        return false;
    }
}





