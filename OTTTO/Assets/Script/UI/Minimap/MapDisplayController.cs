
using UnityEngine;
using UnityEngine.InputSystem;

public class MapDisplayController : MonoBehaviour
{
    [Header("表示するマップ")]
    [SerializeField] private GameObject mapPanel;

    private void Start()
    {
        // ゲーム開始時は非表示
        if (mapPanel != null)
        {
            mapPanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (mapPanel == null || Keyboard.current == null)
        {
            return;
        }

        // Qキーを押している間だけ表示
        bool isPressed = Keyboard.current.qKey.isPressed;

        mapPanel.SetActive(isPressed);
    }
}
