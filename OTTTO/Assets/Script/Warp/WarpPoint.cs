using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarpPoint : MonoBehaviour
{
    public Transform targetPosition; // ワープ先の位置
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // プレイヤーが触れたとき
        {
            other.transform.position = targetPosition.position; // ワープ
        }
    }
}
