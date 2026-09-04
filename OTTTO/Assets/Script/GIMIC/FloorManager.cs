using System.Collections.Generic;
using UnityEngine;

public class FloorManager : MonoBehaviour
{
    [SerializeField]
    private int removeCount = 20;

    void Start()
    {
        List<Transform> floors = new List<Transform>();

        // 子オブジェクトを取得
        foreach (Transform floor in transform)
        {
            floors.Add(floor);
        }

        // 消す枚数が床の数を超えないようにする
        removeCount = Mathf.Min(removeCount, floors.Count);

        // ランダムに選んで削除
        for (int i = 0; i < removeCount; i++)
        {
            int randomIndex = Random.Range(0, floors.Count);

            Destroy(floors[randomIndex].gameObject);

            floors.RemoveAt(randomIndex);
        }
    }
}
