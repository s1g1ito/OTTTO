using UnityEngine;
using UnityEngine.InputSystem;

public class MinimapZoom : MonoBehaviour
{
    [Header("ミニマップ専用カメラ")]
    [SerializeField] private Camera minimapCamera;

    [Header("拡大・縮小設定")]
    [SerializeField] private float zoomSpeed = 2f;
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float maxZoom = 30f;

    private void Update()
    {
        if (minimapCamera == null ||
            Keyboard.current == null ||
            Mouse.current == null)
        {
            return;
        }

        if (!Keyboard.current.qKey.isPressed)
        {
            return;
        }

        float scroll = Mouse.current.scroll.ReadValue().y;

        if (scroll != 0f)
        {
            float zoom = minimapCamera.orthographicSize;
            zoom -= Mathf.Sign(scroll) * zoomSpeed;
            zoom = Mathf.Clamp(zoom, minZoom, maxZoom);
            minimapCamera.orthographicSize = zoom;
        }
    }
}
