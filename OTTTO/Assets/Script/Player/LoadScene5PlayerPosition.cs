using UnityEngine;

public class LoadScene5PlayerPosition : MonoBehaviour
{
    void Start()
    {
        // パズルから戻ってきた場合だけ位置を変更する
        if (PlayerPrefs.GetInt("ReturnFromPuzzle", 0) == 1)
        {
            float x = PlayerPrefs.GetFloat("Scene5_PlayerPosX");
            float y = PlayerPrefs.GetFloat("Scene5_PlayerPosY");
            float z = PlayerPrefs.GetFloat("Scene5_PlayerPosZ");

            float rotY = PlayerPrefs.GetFloat("Scene5_PlayerRotY");

            // 保存した位置へ移動
            transform.position = new Vector3(x, y, z);

            // 保存した向きへ戻す
            transform.rotation = Quaternion.Euler(
                0f,
                rotY,
                0f
            );

            Debug.Log(
                "パズル直前の位置へ戻りました: "
                + transform.position
            );

            // 位置を使ったのでフラグを消す
            PlayerPrefs.SetInt("ReturnFromPuzzle", 0);
            PlayerPrefs.Save();
        }
        else
        {
            // 通常のゲーム開始
            Debug.Log("通常開始なので初期位置のままです");
        }
    }
}