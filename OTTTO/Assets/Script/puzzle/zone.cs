
using UnityEngine;

public class GravityArea : MonoBehaviour
{
    public BallMover ball;

    private bool activated = false;

    private void OnTriggerEnter(Collider other)
    {
        if (activated) return;

        if (other.CompareTag("Player"))
        {
            activated = true;

            Debug.Log("プレイヤーが範囲に入りました！");

            ball.StartMove();
        }
    }
}
