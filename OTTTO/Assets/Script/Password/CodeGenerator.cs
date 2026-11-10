using UnityEngine;

public class CodeGenerator
{
    public static string GeneratedCode;

    public static void Generate()
    {
        if (string.IsNullOrEmpty(GeneratedCode))
        {
            GeneratedCode = Random.Range(1000, 10000).ToString();

            Debug.Log("¡‰ñ‚ÌˆÃØ”Ô†: " + GeneratedCode);
        }
    }
}