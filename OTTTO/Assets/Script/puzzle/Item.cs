using UnityEngine;

public class Item : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // プレイヤーの ItemManager を取得
            ItemManager manager = other.GetComponent<ItemManager>();

            if (manager != null)
            {
                manager.itemCount++;
                manager.UpdateUI();

                // 8個なら puzzle を出す
                if (manager.itemCount == 8 && manager.puzzle != null)
                {
                    manager.puzzle.SetActive(true);
                }
            }

            // アイテムを消す
            Destroy(gameObject);
        }
    }
}
