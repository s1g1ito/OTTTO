using UnityEngine;

public class CodeGenerator : MonoBehaviour
{
    public static string GeneratedCode;

    public static void Generate()
    {
        GeneratedCode = Random.Range(1000, 9999).ToString();
        Debug.Log("¡‰ñ‚ÌˆÃØ”Ô†: " + GeneratedCode);
    }
}
