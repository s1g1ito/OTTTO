using UnityEngine;

public class SlowPlate : MonoBehaviour
{
    // スロープに入ったときの速度
    public float slowSpeed = 3f;

    // プレイヤー本来の速度
    private float normalSpeed;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // PlayerMovementを取得
            PlayerMovement player = other.GetComponent<PlayerMovement>();

            if (player != null)
            {
                // 元の速度を保存
                normalSpeed = player.moveSpeed;

                // 速度を遅くする
                player.moveSpeed = slowSpeed;

                Debug.Log("足が遅くなった！");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement player = other.GetComponent<PlayerMovement>();

            if (player != null)
            {
                // 元の速度に戻す
                player.moveSpeed = normalSpeed;

                Debug.Log("通常の速度に戻った！");
            }
        }
    }
}