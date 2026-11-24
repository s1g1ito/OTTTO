using UnityEngine;

public class CodeWatchTrigger : MonoBehaviour
{
    [Header("Player")]
    public Transform player;

    [Header("Codeを見る距離")]
    public float watchDistance = 5f;

    [Header("Codeを見る角度")]
    public float watchAngle = 30f;

    [Header("必要な視認時間")]
    public float requiredWatchTime = 3f;

    [Header("追跡するEnemyHunter")]
    public EnemyHunter hunter;

    [Header("視界を遮るWall")]
    public LayerMask wallMask;

    private float watchTimer = 0f;
    private bool triggered = false;

    void Start()
    {
        // Playerを自動取得
        if (player == null)
        {
            GameObject obj =
                GameObject.FindGameObjectWithTag("Player");

            if (obj != null)
            {
                player = obj.transform;

                Debug.Log(
                    "CodeWatchTrigger：Playerを取得しました"
                );
            }
            else
            {
                Debug.LogError(
                    "CodeWatchTrigger：Playerタグのオブジェクトが見つかりません！"
                );
            }
        }

        // EnemyHunterを自動取得
        if (hunter == null)
        {
            hunter = Object.FindAnyObjectByType<EnemyHunter>();
        }

        if (hunter != null)
        {
            Debug.Log(
                "CodeWatchTrigger：EnemyHunterを取得しました"
            );
        }
        else
        {
            Debug.LogError(
                "CodeWatchTrigger：EnemyHunterが見つかりません！"
            );
        }
    }

    void Update()
    {
        if (triggered)
            return;

        if (player == null)
            return;

        bool isWatching = IsPlayerWatchingCode();

        if (isWatching)
        {
            watchTimer += Time.deltaTime;

            Debug.Log(
                "Code：Playerが暗証番号を見ています " +
                watchTimer.ToString("F1") +
                " / " +
                requiredWatchTime.ToString("F1") +
                "秒"
            );

            if (watchTimer >= requiredWatchTime)
            {
                ForceHunterChase();
            }
        }
        else
        {
            if (watchTimer > 0f)
            {
                Debug.Log(
                    "Code：Playerが暗証番号から視線を外しました"
                );
            }

            // 3秒経過する前ならリセット
            watchTimer = 0f;
        }
    }

    bool IsPlayerWatchingCode()
    {
        // ----------------------------------------
        // Player → Code
        // ----------------------------------------

        Vector3 direction =
            transform.position - player.position;

        float distance = direction.magnitude;

        // 距離
        if (distance > watchDistance)
            return false;

        // ----------------------------------------
        // 水平方向だけで角度を判定
        // ----------------------------------------

        Vector3 flatDirection =
            new Vector3(
                direction.x,
                0f,
                direction.z
            );

        if (flatDirection.sqrMagnitude < 0.001f)
            return true;

        flatDirection.Normalize();

        Vector3 playerForward =
            player.forward;

        playerForward.y = 0f;

        if (playerForward.sqrMagnitude < 0.001f)
            return false;

        playerForward.Normalize();

        float angle =
            Vector3.Angle(
                playerForward,
                flatDirection
            );

        if (angle > watchAngle)
            return false;

        // ----------------------------------------
        // Wallチェック
        // ----------------------------------------

        Vector3 origin =
            player.position + Vector3.up * 1f;

        Vector3 target =
            transform.position + Vector3.up * 1f;

        Vector3 ray =
            target - origin;

        float rayDistance =
            ray.magnitude;

        if (Physics.Raycast(
            origin,
            ray.normalized,
            rayDistance,
            wallMask))
        {
            return false;
        }

        return true;
    }

    void ForceHunterChase()
    {
        triggered = true;

        Debug.Log("================================");
        Debug.Log("Code：暗証番号を3秒以上見ました！");
        Debug.Log(
            "Code：EnemyHunterへ強制追跡を通知します！"
        );
        Debug.Log(
            "暗証番号 = " +
            CodeGenerator.GeneratedCode
        );
        Debug.Log("================================");

        if (hunter != null)
        {
            hunter.StartChase();
        }
        else
        {
            Debug.LogError(
                "CodeWatchTrigger：EnemyHunterが設定されていません！"
            );
        }
    }
}