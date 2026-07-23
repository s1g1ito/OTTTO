using UnityEngine;
using TMPro;

public class CodeHint : MonoBehaviour
{
    public TMP_Text hintText;

    private void Start()
    {
        hintText.text = CodeGenerator.GeneratedCode;
    }
}
