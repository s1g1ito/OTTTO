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
    [Tooltip("EnemyHunter‚ğ’†S‚Æ‚µ‚½360‹‚Ì”­Œ©‹——£")]
    public float viewDistance = 50f;

    [Tooltip("Player‚ğŒŸ’m‚·‚é‚‚³")]
    public float rayHeight = 1f;

    [Header("‹ŠE‚ğÕ‚éƒŒƒCƒ„[")]
    [Tooltip("Wall‚È‚ÇA‹ŠE‚ğÕ‚éLayer‚¾‚¯‚ğİ’è‚µ‚Ä‚­‚¾‚³‚¢BGround‚Í“ü‚ê‚Ü‚¹‚ñB")]
    public LayerMask visionObstacleMask;

    private NavMeshAgent agent;
    private Transform player;

    private int currentPoint = 0;

    // ˆê“xPlayer‚ğ”­Œ©‚µ‚½‚çtrue
    // ˆÈ~‚Í‰i‰“‚É’ÇÕ‚·‚é
    private bool isChasing = false;


    // ==================================================
    // Start
    // ==================================================

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        agent.speed = moveSpeed;
        agent.stoppingDistance = arriveDistance;


        // ==========================================
        // Playeræ“¾
        // ==========================================

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


        // ==========================================
        // NavMeshŠm”F
        // ==========================================

        if (!agent.isOnNavMesh)
        {
            Debug.LogError(
                "yEnemyHunterzNavMeshã‚É‚¢‚Ü‚¹‚ñI"
            );

            return;
        }


        // ==========================================
        // Å‰‚Íœpœj
        // ==========================================

        isChasing = false;

        if (patrolPoints != null &&
            patrolPoints.Length > 0)
        {
            if (patrolPoints[0] != null)
            {
                agent.isStopped = false;

                agent.SetDestination(
                    patrolPoints[0].position
                );
            }
        }
    }


    // ==================================================
    // Update
    // ==================================================

    void Update()
    {
        // ==========================================
        // Player‚ªæ“¾‚Å‚«‚Ä‚¢‚È‚¢
        // ==========================================

        if (player == null)
        {
            Patrol();

            return;
        }


        // ==========================================
        // Agent‚ª–³Œø
        // ==========================================

        if (agent == null ||
            !agent.enabled)
        {
            return;
        }


        // ==========================================
        // NavMeshã‚É‚¢‚È‚¢
        // ==========================================

        if (!agent.isOnNavMesh)
        {
            return;
        }


        // ==========================================
        // ’ÇÕ’†
        // ==========================================

        if (isChasing)
        {
            // Player‚Ì•ûŒü‚ğŒü‚­
            Vector3 lookDirection =
                player.position -
                transform.position;

            lookDirection.y = 0f;

            if (lookDirection != Vector3.zero)
            {
                Quaternion targetRotation =
                    Quaternion.LookRotation(
                        lookDirection
                    );

                transform.rotation =
                    Quaternion.Slerp(
                        transform.rotation,
                        targetRotation,
                        5f * Time.deltaTime
                    );
            }


            // Player‚ğ‰i‹v’ÇÕ
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


        // ==========================================
        // Player‚ğ”­Œ©‚µ‚Ä‚¢‚È‚¢
        // ¨ œpœj
        // ==========================================

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
    //
    // 360‹‚Ì‰~Œ`”ÍˆÍ
    // ==================================================

    bool CanSeePlayer()
    {
        if (player == null)
        {
            return false;
        }


        // ==========================================
        // Ray‚ÌŠJnˆÊ’u
        // ==========================================

        Vector3 origin =
            transform.position +
            Vector3.up * rayHeight;


        // ==========================================
        // Player‚ÌˆÊ’u
        // ==========================================

        Vector3 target =
            player.position +
            Vector3.up * 1.0f;


        // ==========================================
        // Player‚Ö‚Ì•ûŒü
        // ==========================================

        Vector3 direction =
            target - origin;


        // ==========================================
        // ‹——£
        //
        // Šp“xƒ`ƒFƒbƒN‚Í‚µ‚È‚¢
        // ¨ 360‹‘S•ûŒü
        // ==========================================

        float distance =
            direction.magnitude;


        // ƒfƒoƒbƒO
        Debug.DrawRay(
            origin,
            direction,
            Color.yellow
        );


        // ==========================================
        // ”­Œ©‹——£ƒ`ƒFƒbƒN
        // ==========================================

        if (distance > viewDistance)
        {
            return false;
        }


        // ==========================================
        // Player‚Ü‚ÅRay‚ğ”ò‚Î‚·
        // ==========================================

        Vector3 rayDirection =
            direction.normalized;

        float rayDistance =
            direction.magnitude;


        // ==========================================
        // RaycastAll
        // ==========================================

        RaycastHit[] hits =
            Physics.RaycastAll(
                origin,
                rayDirection,
                rayDistance
            );


        // ==========================================
        // ‹——£‡‚É•À‚×‚é
        // ==========================================

        System.Array.Sort(
            hits,
            (a, b) =>
                a.distance.CompareTo(b.distance)
        );


        // ==========================================
        // RaycastŒ‹‰Ê‚ğŠm”F
        // ==========================================

        foreach (RaycastHit hit in hits)
        {
            GameObject hitObject =
                hit.collider.gameObject;


            string layerName =
                LayerMask.LayerToName(
                    hitObject.layer
                );


            // ==========================================
            // Player
            // ==========================================

            if (hitObject.CompareTag("Player"))
            {
                Debug.Log(
                    "yEnemyHunterzPlayer‚ğ‹”F‚µ‚Ü‚µ‚½I"
                );

                return true;
            }


            // ==========================================
            // Ground
            //
            // Ground‚Í‹ŠE‚ğÕ‚ç‚È‚¢
            // ==========================================

            if (layerName == "Ground")
            {
                continue;
            }


            // ==========================================
            // Wall
            //
            // Wall‚Í‹ŠE‚ğÕ‚é
            // ==========================================

            if (layerName == "Wall")
            {
                Debug.Log(
                    "yEnemyHunterzWall‚É‹ŠE‚ğÕ‚ç‚ê‚Ü‚µ‚½"
                );

                return false;
            }


            // ==========================================
            // ‚»‚Ì‘¼‚ÌCollider
            //
            // –³‹‚µ‚ÄŸ‚ÌCollider‚ğŒ©‚é
            // ==========================================

        }


        return false;
    }


    // ==================================================
    // ’ÇÕŠJn
    //
    // EnemyScout / CodeWatchTrigger‚È‚Ç‚©‚ç
    // ŒÄ‚Ño‚·‚±‚Æ‚ª‚Å‚«‚é
    // ==================================================

    public void StartChase()
    {
        Debug.Log(
            "================================"
        );

        Debug.Log(
            "yEnemyHunterz’ÇÕ’Ê’m‚ğóM‚µ‚Ü‚µ‚½I"
        );


        // ==========================================
        // ‚·‚Å‚É’ÇÕ’†
        // ==========================================

        if (isChasing)
        {
            Debug.Log(
                "yEnemyHunterz‚·‚Å‚É’ÇÕ’†‚Å‚·"
            );

            Debug.Log(
                "================================"
            );

            return;
        }


        // ==========================================
        // NavMeshAgentŠm”F
        // ==========================================

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


        // ==========================================
        // PlayerŠm”F
        // ==========================================

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


        // ==========================================
        // Player‚Ì•ûŒü‚ğŒü‚­
        // ==========================================

        Vector3 lookDirection =
            player.position -
            transform.position;

        lookDirection.y = 0f;

        if (lookDirection != Vector3.zero)
        {
            transform.rotation =
                Quaternion.LookRotation(
                    lookDirection
                );
        }


        // ==========================================
        // Player‚ÖˆÚ“®
        // ==========================================

        agent.isStopped = false;

        agent.SetDestination(
            player.position
        );


        // ==========================================
        // ƒƒO
        // ==========================================

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
            "yEnemyHunterz”­Œ©‹——£ = " +
            viewDistance
        );

        Debug.Log(
            "yEnemyHunterz‰i‹v’ÇÕƒ‚[ƒhŠJn"
        );

        Debug.Log(
            "================================"
        );
    }
}