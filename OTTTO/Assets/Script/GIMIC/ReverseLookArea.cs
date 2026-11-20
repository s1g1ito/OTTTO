using UnityEngine;

public class ReverseLookArea : MonoBehaviour
{
    [SerializeField] private MouseLook mouseLook;

    // エリアに入ったとき
    private void OnTriggerEnter(Collider other)
    {
        PlayerMovement player =
            other.GetComponentInParent<PlayerMovement>();

        if (player != null && mouseLook != null)
        {
            mouseLook.isLookReversed = true;
        }
    }

    // エリアから出たとき
    private void OnTriggerExit(Collider other)
    {
        PlayerMovement player =
            other.GetComponentInParent<PlayerMovement>();

        if (player != null && mouseLook != null)
        {
            mouseLook.isLookReversed = false;
        }
    }
}