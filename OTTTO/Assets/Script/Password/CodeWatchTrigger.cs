using UnityEngine;

public class CodeWatchTrigger : MonoBehaviour
{
    [Header("Player")]
    public Transform player;

    [Header("Player‚ÌƒJƒƒ‰")]
    public Camera playerCamera;

    [Header("Code‚ğŒ©‚é‹——£")]
    public float watchDistance = 5f;

    [Header("•K—v‚È‹”FŠÔ")]
    public float requiredWatchTime = 3f;

    [Header("’ÇÕ‚·‚éEnemyHunter")]
    public EnemyHunter hunter;

    [Header("‹ŠE‚ğÕ‚éWall")]
    public LayerMask wallMask;

    [Header("Code”»’è")]
    public Collider codeCollider;

    private float watchTimer = 0f;
    private bool triggered = false;
    private bool wasWatching = false;


    void Start()
    {
        // ==========================================
        // Playeræ“¾
        // ==========================================

        if (player == null)
        {
            GameObject obj =
                GameObject.FindGameObjectWithTag("Player");

            if (obj != null)
            {
                player = obj.transform;

                Debug.Log(
                    "yCodeWatchzPlayer‚ğæ“¾‚µ‚Ü‚µ‚½"
                );
            }
            else
            {
                Debug.LogError(
                    "yCodeWatchzPlayerƒ^ƒO‚ÌƒIƒuƒWƒFƒNƒg‚ªŒ©‚Â‚©‚è‚Ü‚¹‚ñI"
                );
            }
        }


        // ==========================================
        // Cameraæ“¾
        // ==========================================

        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }

        if (playerCamera != null)
        {
            Debug.Log(
                "yCodeWatchzPlayerƒJƒƒ‰‚ğæ“¾‚µ‚Ü‚µ‚½F" +
                playerCamera.name
            );
        }
        else
        {
            Debug.LogError(
                "yCodeWatchzMain Camera‚ªŒ©‚Â‚©‚è‚Ü‚¹‚ñI"
            );
        }


        // ==========================================
        // EnemyHunteræ“¾
        // ==========================================

        if (hunter == null)
        {
            hunter =
                Object.FindAnyObjectByType<EnemyHunter>();
        }

        if (hunter != null)
        {
            Debug.Log(
                "yCodeWatchzEnemyHunter‚ğæ“¾‚µ‚Ü‚µ‚½F" +
                hunter.name
            );
        }
        else
        {
            Debug.LogError(
                "yCodeWatchzEnemyHunter‚ªŒ©‚Â‚©‚è‚Ü‚¹‚ñI"
            );
        }
    }


    void Update()
    {
        if (triggered)
            return;

        if (player == null)
            return;

        if (playerCamera == null)
            return;


        bool isWatching =
            IsPlayerWatchingCode();


        // ==========================================
        // Code‚ğŒ©‚Ä‚¢‚é
        // ==========================================

        if (isWatching)
        {
            watchTimer += Time.deltaTime;


            if (!wasWatching)
            {
                Debug.Log(
                    "yCodeWatchzPlayer‚ªCode‚ğŒ©n‚ß‚Ü‚µ‚½"
                );

                Debug.Log(
                    "yCodeWatchzˆÃØ”Ô†F" +
                    CodeGenerator.GeneratedCode
                );
            }

            wasWatching = true;


            // 0.5•b‚²‚Æ‚É•\¦
            if (
                Mathf.FloorToInt(watchTimer * 2f) !=
                Mathf.FloorToInt(
                    (watchTimer - Time.deltaTime) * 2f
                )
            )
            {
                Debug.Log(
                    "yCodeWatchz‹”FŠÔ " +
                    watchTimer.ToString("F1") +
                    " / " +
                    requiredWatchTime.ToString("F1") +
                    "•b"
                );
            }


            // ======================================
            // 3•b’B¬
            // ======================================

            if (watchTimer >= requiredWatchTime)
            {
                ForceHunterChase();
            }
        }


        // ==========================================
        // Code‚ğŒ©‚Ä‚¢‚È‚¢
        // ==========================================

        else
        {
            if (wasWatching)
            {
                Debug.Log(
                    "yCodeWatchzPlayer‚ªCode‚©‚ç‹ü‚ğŠO‚µ‚Ü‚µ‚½"
                );

                Debug.Log(
                    "yCodeWatchz‹”Fƒ^ƒCƒ}[‚ğƒŠƒZƒbƒg‚µ‚Ü‚·"
                );
            }

            watchTimer = 0f;
            wasWatching = false;
        }
    }


    // ==================================================
    // Player‚ªCode‚ğŒ©‚Ä‚¢‚é‚©
    // ==================================================

    bool IsPlayerWatchingCode()
    {
        Ray ray =
            playerCamera.ViewportPointToRay(
                new Vector3(0.5f, 0.5f, 0f)
            );

        Debug.DrawRay(
            ray.origin,
            ray.direction * watchDistance,
            Color.cyan
        );

        RaycastHit[] hits =
            Physics.RaycastAll(
                ray,
                watchDistance
            );

        System.Array.Sort(
            hits,
            (a, b) =>
                a.distance.CompareTo(b.distance)
        );

        Debug.Log(
            "yCodeWatchzRaycastƒqƒbƒg”F" +
            hits.Length
        );

        foreach (RaycastHit hit in hits)
        {
            GameObject hitObject =
                hit.collider.gameObject;

            Debug.Log(
                "yCodeWatchzRaycast ¨ " +
                hitObject.name +
                " / Layer = " +
                LayerMask.LayerToName(hitObject.layer)
            );

            // =====================================
            // w’è‚µ‚½Code Collider‚É“–‚½‚Á‚½
            // =====================================
            if (codeCollider != null &&
                hit.collider == codeCollider)
            {
                Debug.Log(
                    "yCodeWatchzš Code‚É“–‚½‚è‚Ü‚µ‚½I"
                );

                return true;
            }

            string layerName =
                LayerMask.LayerToName(hitObject.layer);

            // Ground‚Í–³‹
            if (layerName == "Ground")
            {
                continue;
            }

            // Wall‚Í‹ŠE‚ğÕ‚é
            if (layerName == "Wall")
            {
                Debug.Log(
                    "yCodeWatchzWall‚É‹ŠE‚ğÕ‚ç‚ê‚Ü‚µ‚½"
                );

                return false;
            }

            // Code‚æ‚èè‘O‚É‚ ‚é•ÊCollider
            Debug.Log(
                "yCodeWatchzCode‚æ‚èæ‚É•Ê‚ÌCollider‚É“–‚½‚è‚Ü‚µ‚½F" +
                hitObject.name
            );

            return false;
        }

        Debug.Log(
            "yCodeWatchz‰½‚É‚à“–‚½‚è‚Ü‚¹‚ñ‚Å‚µ‚½"
        );

        return false;
    }


    // ==================================================
    // 3•bŒ©‚½
    // ==================================================

    void ForceHunterChase()
    {
        triggered = true;


        Debug.Log(
            "================================"
        );

        Debug.Log(
            "yCodeWatchzš 3•b’B¬ š"
        );

        Debug.Log(
            "yCodeWatchzˆÃØ”Ô†F" +
            CodeGenerator.GeneratedCode
        );

        Debug.Log(
            "yCodeWatchzEnemyHunter‚Ö‹­§’ÇÕ’Ê’mI"
        );


        if (hunter == null)
        {
            Debug.LogError(
                "yCodeWatchzEnemyHunter‚ªNULL‚Å‚·I"
            );

            return;
        }


        Debug.Log(
            "yCodeWatchz’Ê’mæF" +
            hunter.name
        );


        // ==========================================
        // EnemyHunter‚Ö’Ê’m
        // ==========================================

        hunter.StartChase();


        Debug.Log(
            "yCodeWatchzStartChase()‚ğŒÄ‚Ño‚µ‚Ü‚µ‚½I"
        );

        Debug.Log(
            "================================"
        );
    }
}