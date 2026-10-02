using UnityEngine;

public class Itemtag : MonoBehaviour
{
    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            

            if (Input.GetKeyDown(KeyCode.E)) //Bキーを押すとアイテムを拾う
            {
                Debug.Log("取得しました");

                Destroy(gameObject, 0.2f); //拾ったらそのアイテムを消す
                //Invoke(new(() => { text = string.Empty; }).Method.Name, 2); //取得後2秒経過でメッセージを消す
            }
        }
    }
}
