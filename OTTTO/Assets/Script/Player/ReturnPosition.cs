using UnityEngine;

public class ReturnPosition : MonoBehaviour
{
    public static Vector3 Position;
    public static Quaternion Rotation;
    public static bool HasPosition = false;

    public Transform returnPoint;

    public void SetReturnPosition()
    {
        Position = returnPoint.position;
        Rotation = returnPoint.rotation;
        HasPosition = true;

        Debug.Log("ñﬂÇÈà íuÇï€ë∂ÇµÇ‹ÇµÇΩÅI");
    }
}
