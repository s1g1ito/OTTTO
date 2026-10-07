using UnityEngine;

public class BallMover : MonoBehaviour
{
    public float speed = 5f;
    private bool isMoving = false;

    public void StartMove()
    {
        isMoving = true;
    }

    private void FixedUpdate()
    {
        if (isMoving)
        {
            // ‘O•ûŒü‚É“]‚ª‚·
            transform.Translate(Vector3.back * speed * Time.fixedDeltaTime);
        }
    }
}
