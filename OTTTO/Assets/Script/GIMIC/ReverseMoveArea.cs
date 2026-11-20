using UnityEngine;

public class ReverseMoveArea : MonoBehaviour
{
    // エリアから出た後も操作反転する時間
    [SerializeField] private float reverseTime = 5f;

    // エリアに入ったとき
    private void OnTriggerEnter(Collider other)
    {
        ApplyReverse(other);
    }

    // エリア内にいる間
    private void OnTriggerStay(Collider other)
    {
        ApplyReverse(other);
    }

    private void ApplyReverse(Collider other)
    {
        PlayerMovement player =
            other.GetComponentInParent<PlayerMovement>();

        if (player != null)
        {
            player.ReverseMove(reverseTime);
        }
    }
}