using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class ItemManager : MonoBehaviour
{
    public int itemCount = 0;
    public TMP_Text itemText;

    public GameObject puzzle;

    private void Start()
    {
        UpdateUI();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            itemCount++;
            UpdateUI();
            Destroy(other.gameObject);

            if (itemCount == 8 && puzzle != null)
            {
                puzzle.SetActive(true);
            }
        }
    }

    private void UpdateUI()
    {
        itemText.text = "Item: " + itemCount;
    }
}




