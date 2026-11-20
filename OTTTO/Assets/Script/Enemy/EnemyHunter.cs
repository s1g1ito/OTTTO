using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyHunter : MonoBehaviour
{
    [Header("œpœjİ’è")]
    public Transform[] patrolPoints;
    public float arriveDistance = 0.5f;

    [Header("ˆÚ“®İ’è")]
    public float moveSpeed = 3f;

    [Header("Player”­Œ©İ’è")]
    public float viewDistance = 10f;
    public float viewAngle = 90f;
    public float rayHeight = 1f;

    [Header("’ÇÕİ’è")]
    public float loseDistance = 20f;

    [Header("‹ŠE‚ğÕ‚éƒŒƒCƒ„[")]
    [Tooltip("Wall‚È‚ÇA‹ŠE‚ğÕ‚éLayer‚¾‚¯‚ğİ’è‚µ‚Ä‚­‚¾‚³‚¢BGround‚Í“ü‚ê‚Ü‚¹‚ñB")]
    public LayerMask visionObstacleMask;

    private NavMeshAgent agent;
    private Transform player;

    private int currentPoint = 0;

    // EnemyScout‚©‚ç’Ê’m‚³‚ê‚½‚©A
    // EnemyHunter©g‚ªPlayer‚ğ”­Œ©‚µ‚½‚©
    private bool isChasing = false;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        agent.speed = moveSpeed;
        agent.stoppingDistance = arriveDistance;


        // ==============================
        // Playeræ“¾
        // ==============================

        GameObject obj =
            GameObject.FindGameObjectWithTag("Player");

        if (obj != null)
        {
            player = obj.transform;

            Debug.Log(
                "yEnemyHunterzPlayer‚ğæ“¾‚µ‚Ü‚µ‚½"
            );
        }
        else
        {
            Debug.LogError(
                "yEnemyHunterzPlayerƒ^ƒO‚ÌƒIƒuƒWƒFƒNƒg‚ªŒ©‚Â‚©‚è‚Ü‚¹‚ñI"
            );
        }


        // ==============================
        // NavMeshŠm”F
        // ==============================

        if (!agent.isOnNavMesh)
        {
            Debug.LogError(
                "yEnemyHunterzNavMeshã‚É‚¢‚Ü‚¹‚ñI"
            );

            return;
        }


        // ==============================
        // Å‰‚Íœpœj
        // ==============================

        isChasing = false;

        if (patrolPoints != null &&
            patrolPoints.Length > 0)
        {
            if (patrolPoints[0] != null)
            {
                agent.SetDestination(
                    patrolPoints[0].position
                );
            }
        }
    }


    void Update()
    {
        // Player‚ªæ“¾‚Å‚«‚Ä‚¢‚È‚¢
        if (player == null)
        {
            Patrol();
            return;
        }

        // Agent‚ª–³Œø
        if (agent == null ||
            !agent.enabled)
        {
            return;
        }

        // NavMeshã‚É‚¢‚È‚¢
        if (!agent.isOnNavMesh)
        {
            return;
        }


        // ==========================================
        // ’ÇÕ’†
        // ==========================================

        if (isChasing)
        {
            float distance =
                Vector3.Distance(
                    transform.position,
                    player.position
                );


            // Player‚ª‰“‚­‚É“¦‚°‚½‚ç’ÇÕI—¹
            if (distance > loseDistance)
            {
                StopChase();
                return;
            }


            // Player‚ğ’ÇÕ
            agent.isStopped = false;

            agent.SetDestination(
                player.position
            );

            return;
        }


        // ==========================================
        // ’ÇÕ‚µ‚Ä‚¢‚È‚¢
        // ==========================================

        if (CanSeePlayer())
        {
            Debug.Log(
                "================================"
            );

            Debug.Log(
                "yEnemyHunterz©•ª‚ÅPlayer‚ğ”­Œ©‚µ‚Ü‚µ‚½I"
            );

            Debug.Log(
                "yEnemyHunterz©—Í‚Å’ÇÕ‚ğŠJn‚µ‚Ü‚·"
            );

            Debug.Log(
                "================================"
            );

            StartChase();

            return;
        }


        // Player‚ğ”­Œ©‚µ‚Ä‚¢‚È‚¢‚Ì‚Åœpœj
        Patrol();
    }


    // ==================================================
    // œpœj
    // ==================================================

    void Patrol()
    {
        if (agent == null ||
            !agent.enabled ||
            !agent.isOnNavMesh)
        {
            return;
        }

        if (patrolPoints == null ||
            patrolPoints.Length == 0)
        {
            return;
        }

        if (patrolPoints[currentPoint] == null)
        {
            return;
        }


        if (!agent.pathPending &&
            agent.remainingDistance <= arriveDistance)
        {
            currentPoint =
                (currentPoint + 1) %
                patrolPoints.Length;


            if (patrolPoints[currentPoint] != null)
            {
                agent.isStopped = false;

                agent.SetDestination(
                    patrolPoints[currentPoint].position
                );
            }
        }
    }


    // ==================================================
    // Player‚ğ©•ª‚Å”­Œ©‚Å‚«‚é‚©
    // ==================================================

    bool CanSeePlayer()
    {
        if (player == null)
        {
            return false;
        }


        Vector3 origin =
            transform.position +
            Vector3.up * rayHeight;

        Vector3 target =
            player.position +
            Vector3.up * 1.0f;

        Vector3 direction =
            target - origin;


        Vector3 flatDirection =
            new Vector3(
                direction.x,
                0f,
                direction.z
            );


        float distance =
            flatDirection.magnitude;


        // ‹——£ƒ`ƒFƒbƒN
        if (distance > viewDistance)
        {
            return false;
        }


        // Šp“xƒ`ƒFƒbƒN
        float angle =
            Vector3.Angle(
                transform.forward,
                flatDirection
            );


        if (angle > viewAngle / 2f)
        {
            return false;
        }


        Vector3 rayDirection =
            direction.normalized;

        float rayDistance =
            direction.magnitude;


        Debug.DrawRay(
            origin,
            rayDirection * rayDistance,
            Color.yellow
        );


        // RaycastAll
        RaycastHit[] hits =
            Physics.RaycastAll(
                origin,
                rayDirection,
                rayDistance
            );


        // ‹——£‡‚É•À‚×‚é
        System.Array.Sort(
            hits,
            (a, b) =>
                a.distance.CompareTo(b.distance)
        );


        foreach (RaycastHit hit in hits)
        {
            GameObject hitObject =
                hit.collider.gameObject;


            string layerName =
                LayerMask.LayerToName(
                    hitObject.layer
                );


            // Player
            if (hitObject.CompareTag("Player"))
            {
                Debug.Log(
                    "yEnemyHunterzPlayer‚ğ‹”F‚µ‚Ü‚µ‚½I"
                );

                return true;
            }


            // Ground‚Í–³‹
            if (layerName == "Ground")
            {
                continue;
            }


            // Wall‚Í‹ŠE‚ğÕ‚é
            if (layerName == "Wall")
            {
                Debug.Log(
                    "yEnemyHunterzWall‚É‹ŠE‚ğÕ‚ç‚ê‚Ü‚µ‚½"
                );

                return false;
            }


            // ‚»‚Ì‘¼‚Í–³‹
        }


        return false;
    }


    // ==================================================
    // EnemyScout‚©‚ç’Ê’m‚ğó‚¯æ‚Á‚½‚Æ‚«
    // ==================================================

    public void StartChase()
    {
        Debug.Log(
            "================================"
        );

        Debug.Log(
            "yEnemyHunterz’ÇÕ’Ê’m‚ğóM‚µ‚Ü‚µ‚½I"
        );


        // ‚·‚Å‚É’ÇÕ’†
        if (isChasing)
        {
            Debug.Log(
                "yEnemyHunterz‚·‚Å‚É’ÇÕ’†‚È‚Ì‚Å‰½‚à‚µ‚Ü‚¹‚ñ"
            );

            Debug.Log(
                "================================"
            );

            return;
        }


        // NavMeshAgentŠm”F
        if (agent == null)
        {
            Debug.LogError(
                "yEnemyHunterzNavMeshAgent‚ª‚ ‚è‚Ü‚¹‚ñI"
            );

            return;
        }


        if (!agent.enabled)
        {
            Debug.LogError(
                "yEnemyHunterzNavMeshAgent‚ª–³Œø‚Å‚·I"
            );

            return;
        }


        if (!agent.isOnNavMesh)
        {
            Debug.LogError(
                "yEnemyHunterzNavMeshã‚É‚¢‚Ü‚¹‚ñI"
            );

            return;
        }


        // PlayerŠm”F
        if (player == null)
        {
            Debug.LogError(
                "yEnemyHunterzPlayer‚ªæ“¾‚Å‚«‚Ä‚¢‚Ü‚¹‚ñI"
            );

            return;
        }


        // ==========================================
        // ’ÇÕŠJn
        // ==========================================

        isChasing = true;

        agent.isStopped = false;

        agent.SetDestination(
            player.position
        );


        Debug.Log(
            "yEnemyHunterz’ÇÕŠJn‚µ‚Ü‚µ‚½I"
        );

        Debug.Log(
            "yEnemyHunterz’ÇÕ‘ÎÛ = " +
            player.name
        );

        Debug.Log(
            "yEnemyHunterzPlayerˆÊ’u = " +
            player.position
        );

        Debug.Log(
            "================================"
        );
    }


    // ==================================================
    // ’ÇÕI—¹
    // ==================================================

    void StopChase()
    {
        isChasing = false;


        if (agent != null &&
            agent.enabled &&
            agent.isOnNavMesh)
        {
            agent.ResetPath();
        }


        Debug.Log(
            "yEnemyHunterzPlayer‚ğŒ©¸‚¢‚Ü‚µ‚½BœpœjÄŠJ"
        );
    }
}