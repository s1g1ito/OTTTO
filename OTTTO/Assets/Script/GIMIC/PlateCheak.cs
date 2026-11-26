using System.Collections.Generic;
using UnityEngine;

public class PlateCheak : MonoBehaviour
{
    [Header("開閉する扉")]
    public DoorMove door;

    // 感圧板に乗っている箱のコライダー
    private HashSet<Collider> boxesOnPlate =
        new HashSet<Collider>();

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("KeyBox"))
        {
            return;
        }

        if (boxesOnPlate.Add(other))
        {
            Debug.Log("箱が乗った！");

            if (door != null)
            {
                door.MoveRight();
            }
            else
            {
                Debug.LogWarning(
                    "PlateCheakのDoorが未設定です");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("KeyBox"))
        {
            return;
        }

        boxesOnPlate.Remove(other);

        // 箱がすべて降りたら扉を閉じる
        if (boxesOnPlate.Count == 0)
        {
            Debug.Log("箱がどいた！");

            if (door != null)
            {
                door.MoveBack();
            }
        }
    }

    // 迷路再生成時に感圧板を初期化する
    public void ResetPlate()
    {
        boxesOnPlate.Clear();

        if (door != null)
        {
            door.MoveBack();
        }
    }
}