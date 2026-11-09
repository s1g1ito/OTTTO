using UnityEngine;

public class GameStartCode : MonoBehaviour
{
    private void Awake()
    {
        CodeGenerator.Generate();
    }
}