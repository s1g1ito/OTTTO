using UnityEngine;
using UnityEngine.SceneManagement;

public class move : MonoBehaviour
{

    internal string text; //表示するテキストを入れる変数

    private float speed = 3.0f;
   
    void Start()
    {
        DontDestroyOnLoad(gameObject); //シーンを切り替えても削除しない

        text = string.Empty; //テキストを空の文字列で初期化

    }

    void Update()
    {
        float moveX = Input.GetAxis("Horizontal") * Time.deltaTime * speed;
        float moveZ = Input.GetAxis("Vertical") * Time.deltaTime * speed;
        transform.position = new Vector3(
            transform.position.x + moveX,
            transform.position.y,
            transform.position.z + moveZ
            );
    }
    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Item")
        {
            text = "B 拾う";

            if (Input.GetKeyDown(KeyCode.B)) //Bキーを押すとアイテムを拾う
            {
                Debug.Log("取得しました");
                text = collision.gameObject.name + "を取得した";
                Destroy(collision.gameObject); //拾ったらそのアイテムを消す
                //Invoke(new(() => { text = string.Empty; }).Method.Name, 2); //取得後2秒経過でメッセージを消す
            }
        }
    }
   
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.name == "puzzle")
        {
            SceneManager.LoadScene("puzzle");

            Debug.Log("puzzleに移動しました");

            this.transform.position = new Vector3(46f, 1f, 45f);

            Debug.Log("移動しました");

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }
    }
}
