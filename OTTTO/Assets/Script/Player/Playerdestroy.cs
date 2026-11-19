
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class KillOnTouch : MonoBehaviour
{
    [Header("触れたらリスポーンするオブジェクト")]
    [SerializeField] private GameObject[] deadlyObjects;

    private bool isRespawning = false;

    private void OnTriggerEnter(Collider other)
    {
        foreach (GameObject obj in deadlyObjects)
        {
            if (obj != null && other.gameObject == obj)
            {
                if (!isRespawning)
                {
                    StartCoroutine(Respawn());
                }

                return;
            }
        }
    }

    private IEnumerator Respawn()
    {
        isRespawning = true;

        // 現在のシーンを再読み込み
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        yield return null;
    }
}
