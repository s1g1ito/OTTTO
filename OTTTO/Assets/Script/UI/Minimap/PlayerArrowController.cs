using UnityEngine;

public class PlayerArrowController : MonoBehaviour
{
    [SerializeField] private Transform player;

    private void Update()
    {
        if (player == null)
        {
            return;
        }

        // UIã‚Ì–îˆó‚ğƒvƒŒƒCƒ„[‚ÌY‰ñ“]‚É‡‚í‚¹‚é
        transform.localRotation = Quaternion.Euler(0f, 0f, -player.eulerAngles.y);
    }
}