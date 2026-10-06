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

  

    public void UpdateUI()
    {
        itemText.text = "Item: " + itemCount;
    }
}




