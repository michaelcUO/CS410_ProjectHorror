using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class EnemyFollowWhenNotSeen : MonoBehaviour
{
    public Transform player;             // Assign your player object in the Inspector
    public float detectionDistance = 20f;
    public float stopDistance = 1.5f;
    public float fieldOfView = 60f;      // Angle of player vision

    private Animator animator;
    private NavMeshAgent agent;

    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (player == null) return;

        Vector3 toEnemy = (transform.position - player.position).normalized;
        float dot = Vector3.Dot(player.forward, toEnemy);

        bool isPlayerLooking = dot > Mathf.Cos(fieldOfView * Mathf.Deg2Rad);
        float distance = Vector3.Distance(transform.position, player.position);

        if (!isPlayerLooking && distance < detectionDistance)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
            animator.SetBool("IsWalking", true);
        }
        else
        {
            agent.isStopped = true;
            animator.SetBool("IsWalking", false);
        }

        // Rotate to face the player
        Vector3 lookPos = new Vector3(player.position.x, transform.position.y, player.position.z);
        transform.LookAt(lookPos);
    }
}