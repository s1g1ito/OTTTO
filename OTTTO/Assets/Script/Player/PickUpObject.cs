using UnityEngine;
using UnityEngine.InputSystem;

public class PickUpObject : MonoBehaviour
{
    public Transform holdPoint;
    public float range = 2f;

    private GameObject heldObject;
    private InputSystem_Actions input;

    void Awake()
    {
        input = new InputSystem_Actions();
    }

    void OnEnable()
    {
        input.Player.Enable();
        input.Player.Catch.performed += OnCatch;
    }

    void OnDisable()
    {
        input.Player.Catch.performed -= OnCatch;
        input.Player.Disable();
    }

    void OnCatch(InputAction.CallbackContext context)
    {
        if (heldObject == null)
        {
            TryPickUp();
        }
        else
        {
            Drop();
        }
    }

    void TryPickUp()
    {
        // カメラ or プレイヤーの前方向
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * range, Color.red, 1f);

        if (Physics.Raycast(ray, out hit, range))
        {
            if (hit.collider.CompareTag("Item"))
            {
                heldObject = hit.collider.gameObject;

                // 持つ
                heldObject.transform.SetParent(holdPoint);
                heldObject.transform.localPosition = Vector3.zero;
                heldObject.transform.localRotation = Quaternion.identity;
                Debug.DrawRay(ray.origin, ray.direction * range, Color.red, 2f);

                Rigidbody rb = heldObject.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = true;
                    rb.useGravity = false;
                }
            }
        }
    }

    void Drop()
    {
        heldObject.transform.SetParent(null);

        Rigidbody rb = heldObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;

            // 少し前にポイっと落とす
            rb.AddForce(transform.forward * 2f, ForceMode.Impulse);
        }

        heldObject = null;
    }
}