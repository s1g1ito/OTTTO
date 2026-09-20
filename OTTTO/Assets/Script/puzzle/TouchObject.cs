using UnityEngine;

public class TouchObject : MonoBehaviour
{
    public GameObject gameObject;  // Cube（プレイヤー）を指定する
    public GameObject target;      // Targetを指定する

    void OnCollisionEnter(Collision col)
    {
        // Cubeに触れた場合の処理
        if (col.gameObject.name == "9669202_1")
        {
            // Targetの位置にSphere（アイテム）を移動
            this.transform.position = new Vector3(target.transform.position.x, 0.5f, target.transform.position.z);

            // Cubeの子オブジェクトとして設定
            transform.SetParent(gameObject.transform);
        }
    }
}
