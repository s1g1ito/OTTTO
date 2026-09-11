using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class NumberPanel : MonoBehaviour
{
    [SerializeField] TMP_Text numberText;
    public int number = 0;

    public void OnClick()
    {
        number++;

        if (number >= 10)
        {
            number = 0;
        }

        numberText.text = number.ToString();
    }
}
