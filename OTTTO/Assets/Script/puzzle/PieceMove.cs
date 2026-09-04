using UnityEngine;

public class PieceMove : MonoBehaviour
{

    private void OnMouseDown()

    {

        PieceMoving();

    }
    void PieceMoving()

    {

        //上にRayを飛ばす。

        RaycastHit2D hitUp = Physics2D.Raycast(transform.position + Vector3.up, Vector2.up, 0.1f);

        if (!hitUp)

        {

            transform.position += new Vector3(0, 1f, 0);

        }

        //下にRayを飛ばす。

        RaycastHit2D hitDown = Physics2D.Raycast(transform.position + Vector3.down, Vector2.down, 0.1f);

        if (!hitDown)

        {

            transform.position -= new Vector3(0, 1f, 0);

        }

        //右にRayを飛ばす。

        RaycastHit2D hitRight = Physics2D.Raycast(transform.position + Vector3.right, Vector2.right, 0.1f);

        if (!hitRight)

        {

            transform.position += new Vector3(1, 0, 0);

        }

        //左にRayを飛ばす。

        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position + Vector3.left, Vector2.left, 0.1f);

        if (!hitLeft)

        {

            transform.position -= new Vector3(1, 0, 0);
        }

    }
   
}
