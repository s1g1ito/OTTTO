using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeWarp : MonoBehaviour
{
  
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.name == "Zoon")
        {
            this.transform.position = new Vector3(40f, 0f, 44f);
        }
    }
}