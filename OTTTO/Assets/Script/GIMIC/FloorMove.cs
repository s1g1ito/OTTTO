using System.Collections;
using UnityEngine;

public class FloorMove : MonoBehaviour
{
    public GameObject[] floors;

    void Start()
    {
        StartCoroutine(FloorActive());
    }

    IEnumerator FloorActive()
    {
        while (true)
        {
            // 3ïbï\é¶
            foreach (GameObject floor in floors)
            {
                floor.SetActive(true);
            }

            yield return new WaitForSeconds(3.0f);

            // 3ïbè¡Ç∑
            foreach (GameObject floor in floors)
            {
                floor.SetActive(false);
            }

            yield return new WaitForSeconds(3.0f);
        }
    }
}
