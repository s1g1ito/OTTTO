using TMPro;
using UnityEngine;

public class CodeHint : MonoBehaviour
{
    public TMP_Text hintText;

    public void Start()
    {
        hintText.text = CodeGenerator.GeneratedCode;
    }
}
