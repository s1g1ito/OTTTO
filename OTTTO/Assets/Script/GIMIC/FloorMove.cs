using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorMove : MonoBehaviour
{
    public GameObject[] floors;

    // 落とし穴は5か所
    public int holeCount = 5;

    // 消える時間
    public float hideTime = 3f;

    // 表示する時間
    public float showTime = 3f;

    private List<int> holeIndexes = new List<int>();

    void Start()
    {
        SetRandomHoles();

        StartCoroutine(FloorActive());
    }

    void SetRandomHoles()
    {
        if (floors == null || floors.Length < holeCount)
        {
            Debug.LogError("床を5個以上登録してください！");
            return;
        }

        // 全部表示
        foreach (GameObject floor in floors)
        {
            if (floor != null)
            {
                floor.SetActive(true);
            }
        }

        // 床の番号を作る
        List<int> indexes = new List<int>();

        for (int i = 0; i < floors.Length; i++)
        {
            if (floors[i] != null)
            {
                indexes.Add(i);
            }
        }

        // ランダムに並び替える
        for (int i = 0; i < indexes.Count; i++)
        {
            int randomIndex = Random.Range(i, indexes.Count);

            int temp = indexes[i];
            indexes[i] = indexes[randomIndex];
            indexes[randomIndex] = temp;
        }

        // ランダムに5か所を選ぶ
        holeIndexes.Clear();

        for (int i = 0; i < holeCount; i++)
        {
            holeIndexes.Add(indexes[i]);

            Debug.Log("落とし穴：" + floors[indexes[i]].name);
        }
    }

    IEnumerator FloorActive()
    {
        while (true)
        {
            // 5か所を消す
            foreach (int index in holeIndexes)
            {
                floors[index].SetActive(false);
            }

            // 3秒消える
            yield return new WaitForSeconds(hideTime);

            // 5か所を表示
            foreach (int index in holeIndexes)
            {
                floors[index].SetActive(true);
            }

            // 3秒表示
            yield return new WaitForSeconds(showTime);
        }
    }
}

