using UnityEngine;

public class BallFollow : MonoBehaviour
{
    public Transform player;   // プレイヤーの Transform
    public float speed = 5f;   // 追いかける速度

    private void Update()
    {
        if (player == null) return;

        // プレイヤーの方向へ向かう
        Vector3 direction = (player.position - transform.position).normalized;

        // ボールを移動させる
        transform.position += direction * speed * Time.deltaTime;

        // ボールを回転させて転がっているように見せる
        transform.Rotate(Vector3.right * speed * 10f * Time.deltaTime);
    }
}
