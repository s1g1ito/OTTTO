using UnityEngine;

public class CodeGenerator
{
    public static string GeneratedCode;

    public static void Generate()
    {
        GeneratedCode = Random.Range(1000, 10000).ToString();

        Debug.Log("¡‰ñ‚ÌˆÃØ”Ô†: " + GeneratedCode);
    }
}