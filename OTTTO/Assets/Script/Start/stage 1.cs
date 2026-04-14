using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement; //Ç±ÇÍïKê{Ç≈Ç∑

public class Scene : MonoBehaviour
{

    public void SceneChange()
    {
        SceneManager.LoadScene("Stage");
    }


}