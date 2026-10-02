using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ItemManager : MonoBehaviour
{
    private int count = 0;
    private GameObject scoreText;

    void Start()
    {
        scoreText = GameObject.Find("ScoreText");

    }

   
    void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("æ“¾‚µ‚Ü‚µ‚½");
           
            scoreText.GetComponent<ScoreManager>().score = scoreText.GetComponent<ScoreManager>().score + 1;

            Debug.Log("ƒJƒEƒ“ƒg‚ª‚P‘‚¦‚½");

            Destroy(this.gameObject);



        }
    }
}




