using UnityEngine;
using UnityEngine.AI;

public class EnemyScout : MonoBehaviour
{
    [Header("‹”Fİ’è")]
    public float viewDistance = 10f;
    public float viewAngle = 90f;
    public float rayHeight = 1f;

    [Header("œpœjİ’è")]
    public Transform[] patrolPoints;
    public float arriveDistance = 0.5f;

    [Header("’Ê’m‚·‚é“GB")]
    public EnemyHunter hunter;

    private NavMeshAgent agent;
    private Transform player;
    private int currentPoint = 0;
    private bool foundPlayer = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        GameObject obj =
            GameObject.FindGameObjectWithTag("Player");

        if (obj != null)
            player = obj.transform;

        if (patrolPoints.Length > 0)
            agent.SetDestination(patrolPoints[0].position);
    }

    void Update()
    {
        if (player == null) return;

        // Player‚ğ”­Œ©‚µ‚½‚©Šm”F
        if (!foundPlayer && CanSeePlayer())
        {
            foundPlayer = true;

            if (hunter != null)
                hunter.StartChase();

            Debug.Log("“GA‚ªPlayer‚ğ”­Œ©I");
        }

        // “GA‚Í”­Œ©Œã‚àœpœj‚ğ‘±‚¯‚é
        Patrol();
    }

    void Patrol()
    {
        if (patrolPoints == null ||
            patrolPoints.Length == 0) return;

        if (!agent.pathPending &&
            agent.remainingDistance <= arriveDistance)
        {
            currentPoint =
                (currentPoint + 1) % patrolPoints.Length;

            agent.SetDestination(
                patrolPoints[currentPoint].position);
        }
    }

    bool CanSeePlayer()
    {
        Vector3 origin =
            transform.position + Vector3.up * rayHeight;

        Vector3 direction =
            player.position - origin;

        Vector3 flatDirection =
            new Vector3(direction.x, 0, direction.z);

        if (flatDirection.magnitude > viewDistance)
            return false;

        if (Vector3.Angle(
            transform.forward, flatDirection) > viewAngle)
            return false;

        if (Physics.Raycast(
            origin,
            direction.normalized,
            out RaycastHit hit,
            viewDistance))
        {
            return hit.collider.CompareTag("Player");
        }

        return false;
    }
}
