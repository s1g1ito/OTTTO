
using UnityEngine;
using UnityEngine.UI;

public class FogOfWar : MonoBehaviour
{
    [Header("参照")]
    [SerializeField] private Transform player;
    [SerializeField] private Camera minimapCamera;
    [SerializeField] private RawImage fogOverlay;

    [Header("探索設定")]
    [SerializeField] private int textureSize = 500;
    [SerializeField] private float revealRadius = 2f;

    [Header("ステージ全体の探索記録範囲")]
    [SerializeField] private Vector2 worldMin = new Vector2(-100f, -100f);
    [SerializeField] private Vector2 worldMax = new Vector2(100f, 100f);

    private Texture2D fogTexture;
    private Color32[] fogPixels;

    private void Start()
    {
        if (player == null || minimapCamera == null || fogOverlay == null)
        {
            Debug.LogError("FogOfWarの参照が設定されていません。");
            enabled = false;
            return;
        }

        fogTexture = new Texture2D(
            textureSize,
            textureSize,
            TextureFormat.RGBA32,
            false
        );

        fogTexture.wrapMode = TextureWrapMode.Clamp;
        fogTexture.filterMode = FilterMode.Bilinear;

        fogPixels = new Color32[textureSize * textureSize];

        for (int i = 0; i < fogPixels.Length; i++)
        {
            fogPixels[i] = new Color32(0, 0, 0, 255);
        }

        fogTexture.SetPixels32(fogPixels);
        fogTexture.Apply();

        fogOverlay.texture = fogTexture;
        fogOverlay.color = Color.white;
    }

    private void LateUpdate()
    {
        RevealAroundPlayer();
        UpdateMapView();
    }

    private void RevealAroundPlayer()
    {
        // プレイヤーの世界座標をテクスチャ座標に変換
        float normalizedX = Mathf.InverseLerp(
            worldMin.x, worldMax.x, player.position.x
        );

        float normalizedY = Mathf.InverseLerp(
            worldMin.y, worldMax.y, player.position.z
        );

        int centerX = Mathf.RoundToInt(normalizedX * (textureSize - 1));
        int centerY = Mathf.RoundToInt(normalizedY * (textureSize - 1));

        float pixelsPerUnitX =
            textureSize / (worldMax.x - worldMin.x);

        float pixelsPerUnitY =
            textureSize / (worldMax.y - worldMin.y);

        int radiusX = Mathf.CeilToInt(revealRadius * pixelsPerUnitX);
        int radiusY = Mathf.CeilToInt(revealRadius * pixelsPerUnitY);

        for (int y = -radiusY; y <= radiusY; y++)
        {
            for (int x = -radiusX; x <= radiusX; x++)
            {
                float dx = x / pixelsPerUnitX;
                float dy = y / pixelsPerUnitY;

                if (dx * dx + dy * dy > revealRadius * revealRadius)
                {
                    continue;
                }

                int px = centerX + x;
                int py = centerY + y;

                if (px < 0 || px >= textureSize ||
                    py < 0 || py >= textureSize)
                {
                    continue;
                }

                fogPixels[py * textureSize + px] =
                    new Color32(0, 0, 0, 0);
            }
        }

        fogTexture.SetPixels32(fogPixels);
        fogTexture.Apply();
    }

    private void UpdateMapView()
    {
        float viewHeight = minimapCamera.orthographicSize * 2f;
        float viewWidth = viewHeight * minimapCamera.aspect;

        float worldWidth = worldMax.x - worldMin.x;
        float worldHeight = worldMax.y - worldMin.y;

        Vector3 cameraPos = minimapCamera.transform.position;

        float u = (cameraPos.x - worldMin.x) / worldWidth;
        float v = (cameraPos.z - worldMin.y) / worldHeight;

        float uvWidth = viewWidth / worldWidth;
        float uvHeight = viewHeight / worldHeight;

        fogOverlay.uvRect = new Rect(
            u - uvWidth * 0.5f,
            v - uvHeight * 0.5f,
            uvWidth,
            uvHeight
        );
    }

    private void OnDestroy()
    {
        if (fogTexture != null)
        {
            Destroy(fogTexture);
        }
    }
}
