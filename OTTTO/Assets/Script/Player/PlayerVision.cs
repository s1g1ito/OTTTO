using UnityEngine;
using UnityEngine.UI;

public class PlayerVision : MonoBehaviour
{
    public Image blackScreen;

    public void SetDark(bool isDark)
    {
        blackScreen.enabled = isDark;
    }
}
