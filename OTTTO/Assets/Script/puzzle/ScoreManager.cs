using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    // スコアを表示するText
    public TextMeshProUGUI scoreText;

    // 現在のスコア
    public int score = 0;

    void Start()
    {
        // 最初のスコアを0にする
        scoreText.text = "0";
    }

    void Update()
    {
        // スコアを画面に表示する
        scoreText.text = score.ToString();
    }
}