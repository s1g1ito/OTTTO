using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    private TextMeshPro scoreText;
    public int score = 0;

    void Start()
    {
        scoreText = GetComponentInChildren<TextMeshPro>();
        scoreText.text = "0";
    }

    void Update()
    {
        scoreText.text = score.ToString();
    }
}
