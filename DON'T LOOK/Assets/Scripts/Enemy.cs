using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

// Controls enemy AI behavior: follows the player when not being watched,
// stops when looked at, and triggers a game over attack if close enough.
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
public class EnemyFollowWhenNotSeen : MonoBehaviour
{
    public Transform player; // Player's transform to track movement and position
    // public Camera playerCamera; // Main camera to control view snapping during attack
    public MonoBehaviour playerLookScript; // Reference to player's camera control script (to disable on attack)
    public float detectionDistance = 20f; // Max range for detecting the player
    public float fieldOfView = 60f; // Player's field of view angle for determining line of sight

    private Animator animator; // Animator for controlling enemy animations
    private NavMeshAgent agent; // NavMeshAgent for pathfinding

    /// <summary>
    /// Initialize references to components and set stopping distance
    /// </summary>
    void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
    }

    /// <summary>
    /// Main enemy behavior loop: checks visibility, movement, and attack triggers
    /// </summary>
    void Update()
    {
        if (player == null) return;

        // Check if player is looking at enemy using dot product
        Vector3 toEnemy = (transform.position - player.position).normalized;
        float dot = Vector3.Dot(player.forward, toEnemy);
        bool isPlayerLooking = dot > Mathf.Cos(fieldOfView * Mathf.Deg2Rad);
        float distance = Vector3.Distance(transform.position, player.position);

        Debug.DrawRay(player.position, player.forward * 2f, Color.green);
        Debug.DrawRay(player.position, toEnemy * 2f, Color.red);

        Debug.Log($"Distance: {distance}, isPlayerLooking: {isPlayerLooking}");

        // If player is not looking and enemy is in range, move toward player
        // Check if the player is not looking at the enemy and the enemy is within detection range
        if (!isPlayerLooking && distance < detectionDistance)
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);
            animator.SetBool("IsWalking", true);
        }
        else
        {
            // If player is looking or too far, stop moving
            agent.isStopped = true;
            animator.SetBool("IsWalking", false);
        }

        // Ensure enemy is always facing the player horizontally
        Vector3 lookPos = new Vector3(player.position.x, transform.position.y, player.position.z);
        transform.LookAt(lookPos);
    }
}