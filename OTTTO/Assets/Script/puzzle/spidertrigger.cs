
using UnityEngine;

public class spidertrigger : MonoBehaviour
{
    public spaiderchase ball;

    // 追加するボール
    public spaiderchase[] balls;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("何かが範囲に入りました");

        if (other.CompareTag("Player"))
        {
            Debug.Log("Playerを検知しました！");

            // 元のボールを起動
            if (ball != null)
            {
                ball.Activate();
            }

            // 追加したボールをすべて起動
            foreach (spaiderchase b in balls)
            {
                if (b != null)
                {
                    b.Activate();
                }
            }
        }
    }
}
