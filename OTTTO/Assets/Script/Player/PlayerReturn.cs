using UnityEngine;

public class PlayerReturn : MonoBehaviour
{
    void Start()
    {
        if (ReturnPosition.HasPosition)
        {
            transform.position = ReturnPosition.Position;
            transform.rotation = ReturnPosition.Rotation;

            ReturnPosition.HasPosition = false;

            Debug.Log("ReturnPointÇ…ñﬂÇËÇ‹ÇµÇΩÅI");
        }
    }
}
