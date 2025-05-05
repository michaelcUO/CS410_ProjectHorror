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
    public Camera playerCamera; // Reference to the player's camera for line-of-sight and FOV check
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
        animator = GetComponent<Animator>(); // Get the Animator component for controlling animations/
        agent = GetComponent<NavMeshAgent>(); // Get the NavMeshAgent component for pathfinding.
    }

    /// <summary>
    /// Main enemy behavior loop: checks visibility, movement, and attack triggers
    /// </summary>
    void Update()
    {
        if (player == null) return; // Ensure player reference is set before proceeding.

        // Check if player is looking at enemy using dot product
        // Vector3 toEnemy = (transform.position - player.position).normalized; // Direction from player to enemy.
        // float dot = Vector3.Dot(player.forward, toEnemy); // Dot product to determine if player is looking at enemy.
        // bool isPlayerLooking = dot > Mathf.Cos(fieldOfView * Mathf.Deg2Rad); // Check if player is looking at enemy based on field of view.

        bool isPlayerLooking = false;
        Vector3 cameraOrigin = playerCamera.transform.position;
        Vector3 directionToEnemy = (transform.position - cameraOrigin).normalized;

        float angleToEnemy = Vector3.Angle(playerCamera.transform.forward, directionToEnemy);
        if (angleToEnemy < fieldOfView / 2f)
        {
            RaycastHit hit;
            if (Physics.Raycast(cameraOrigin, directionToEnemy, out hit, detectionDistance))
            {
                if (hit.transform == transform)
                {
                    isPlayerLooking = true;
                }
            }
        }
        
        float distance = Vector3.Distance(transform.position, player.position); // Distance from player to enemy.

        Vector3 toEnemy = (transform.position - player.position).normalized; // Direction from player to enemy.
        Debug.DrawRay(player.position, player.forward * 2f, Color.green); // Draw ray from player to show direction of view.
        Debug.DrawRay(player.position, toEnemy * 2f, Color.red); // Draw ray from player to enemy to show direction to enemy.
        Debug.Log($"Distance: {distance}, isPlayerLooking: {isPlayerLooking}"); // Log distance and visibility status for debugging.

        // If player is not looking and enemy is in range, move toward player
        // Check if the player is not looking at the enemy and the enemy is within detection range
        if (!isPlayerLooking && distance < detectionDistance)
        {
            agent.isStopped = false; // Enable movement.
            agent.SetDestination(player.position); // Set the destination to the player's position.
            animator.SetBool("IsWalking", true); // Set walking animation to true.
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