using UnityEngine;
using UnityEngine.InputSystem;

public class GrabSystem : MonoBehaviour
{
    // 掴める距離
    public float grabDistance = 3f;

    // プレイヤーのカメラ
    public Camera playerCamera;

    // 掴んだ物を持つ位置
    public Transform holdPoint;

    // 現在持っている物
    private GameObject heldObject;

    void Update()
    {
        // Eキーを押した
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            // 何も持っていないなら掴む
            if (heldObject == null)
            {
                TryGrab();
            }
            // 何か持っているなら離す
            else
            {
                DropObject();
            }
        }
    }

    void TryGrab()
    {
        Ray ray = new Ray(
            playerCamera.transform.position,
            playerCamera.transform.forward
        );

        RaycastHit hit;

        // 前方に物があるか確認
        if (Physics.Raycast(ray, out hit, grabDistance))
        {
            // KeyBoxタグの物だけ掴む
            if (hit.collider.CompareTag("KeyBox"))
            {
                GrabObject(hit.collider.gameObject);
            }
            else
            {
                Debug.Log("これはKeyBoxではありません");
            }
        }
    }

    void GrabObject(GameObject obj)
    {
        heldObject = obj;

        // 箱を持つ位置に移動
        heldObject.transform.position = holdPoint.position;

        // 箱を持つ位置の子にする
        heldObject.transform.SetParent(holdPoint);

        // Rigidbodyを取得
        Rigidbody rb = heldObject.GetComponent<Rigidbody>();

        if (rb != null)
        {
            // 持っている間は物理演算を止める
            rb.isKinematic = true;
        }

        Debug.Log("KeyBoxを掴みました！");
    }

    void DropObject()
    {
        // 親子関係を解除
        heldObject.transform.SetParent(null);

        // Rigidbodyを取得
        Rigidbody rb = heldObject.GetComponent<Rigidbody>();

        if (rb != null)
        {
            // 物理演算を再開
            rb.isKinematic = false;
        }

        Debug.Log("KeyBoxを離しました！");

        heldObject = null;
    }
}