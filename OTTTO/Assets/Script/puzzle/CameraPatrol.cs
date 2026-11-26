using UnityEngine;

public class CameraPatrol : MonoBehaviour
{
    [Header("移動設定")]
    public float moveSpeed = 3f;

    [Header("移動する方向")]
    public Vector3 moveDirection = Vector3.right;

    [Header("移動範囲")]
    public float moveDistance = 5f;

    private Vector3 startPosition;

    void Start()
    {
        // ゲーム開始時の位置を保存
        startPosition = transform.position;
    }

    void Update()
    {
        // 指定した方向に往復移動
        float offset = Mathf.PingPong(Time.time * moveSpeed, moveDistance * 2f)
                      - moveDistance;

        transform.position = startPosition + moveDirection.normalized * offset;
    }
}