using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemLogger : MonoBehaviour
{
    public GameObject target;
    public Text scoreLabel;
    int sc_count;
    void Start()
    {
        sc_count = 0;
        scoreLabel.text = sc_count.ToString();
    }


    void OnTriggerEnter(Collider hit)
    {
        if (hit.CompareTag("Player"))
        {

            Destroy(gameObject);
           
        }
    }
}
