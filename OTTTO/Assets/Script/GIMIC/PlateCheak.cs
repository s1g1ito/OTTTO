using UnityEngine;

public class PlateCheak : MonoBehaviour
{
    public DoorMove door;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("KeyBox"))
        {
            Debug.Log("箱が乗った！");

            // Doorを右に5動かす
            door.MoveRight();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("KeyBox"))
        {
            Debug.Log("箱がどいた！");

            // Doorを元の位置に戻す
            door.MoveBack();
        }
    }
}