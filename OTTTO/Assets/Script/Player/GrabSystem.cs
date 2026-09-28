using UnityEngine;
using UnityEngine.InputSystem;

public class GrabSystem : MonoBehaviour
{
    // ==============================
    // 掴む設定
    // ==============================

    // 掴める距離
    public float grabDistance = 3f;

    // プレイヤーのカメラ
    public Camera playerCamera;

    // 掴んだ物を持つ位置
    public Transform holdPoint;


    // ==============================
    // 現在の状態
    // ==============================

    // 現在持っている物
    private GameObject heldObject;

    // プレイヤーのAnimator
    private Animator animator;

    // PlayerのCollider
    private Collider playerCollider;

    // 持っている箱のCollider
    private Collider[] heldObjectColliders;


    // ==============================
    // Start
    // ==============================

    void Start()
    {
        // Playerの子にあるAnimatorを取得
        animator = GetComponentInChildren<Animator>();

        // Player自身のColliderを取得
        playerCollider = GetComponent<Collider>();
    }


    // ==============================
    // Update
    // ==============================

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


    // ==============================
    // 箱を探す
    // ==============================

    void TryGrab()
    {
        // カメラから前方にRayを飛ばす
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


    // ==============================
    // 箱を掴む
    // ==============================

    void GrabObject(GameObject obj)
    {
        heldObject = obj;


        // ------------------------------
        // 箱を持つ位置に移動
        // ------------------------------

        heldObject.transform.position = holdPoint.position;


        // ------------------------------
        // HoldPointの子にする
        // ------------------------------

        heldObject.transform.SetParent(holdPoint);


        // ------------------------------
        // Rigidbody
        // ------------------------------

        Rigidbody rb = heldObject.GetComponent<Rigidbody>();

        if (rb != null)
        {
            // 持っている間は物理演算を止める
            rb.isKinematic = true;
        }


        // ------------------------------
        // Playerと箱の衝突を無効にする
        // ------------------------------

        heldObjectColliders =
            heldObject.GetComponentsInChildren<Collider>();

        foreach (Collider boxCollider in heldObjectColliders)
        {
            if (playerCollider != null)
            {
                Physics.IgnoreCollision(
                    playerCollider,
                    boxCollider,
                    true
                );
            }
        }


        // ------------------------------
        // Animator
        // ------------------------------

        animator.SetBool("IsHolding", true);


        Debug.Log("KeyBoxを掴みました！");
    }


    // ==============================
    // 箱を離す
    // ==============================

    void DropObject()
    {
        // ------------------------------
        // 親子関係を解除
        // ------------------------------

        heldObject.transform.SetParent(null);


        // ------------------------------
        // Playerと箱の衝突を元に戻す
        // ------------------------------

        if (heldObjectColliders != null)
        {
            foreach (Collider boxCollider in heldObjectColliders)
            {
                if (playerCollider != null)
                {
                    Physics.IgnoreCollision(
                        playerCollider,
                        boxCollider,
                        false
                    );
                }
            }
        }


        // ------------------------------
        // Rigidbody
        // ------------------------------

        Rigidbody rb = heldObject.GetComponent<Rigidbody>();

        if (rb != null)
        {
            // 物理演算を再開
            rb.isKinematic = false;
        }


        // ------------------------------
        // Animator
        // ------------------------------

        animator.SetBool("IsHolding", false);


        Debug.Log("KeyBoxを離しました！");


        // ------------------------------
        // 持っている物をリセット
        // ------------------------------

        heldObject = null;
        heldObjectColliders = null;
    }
}