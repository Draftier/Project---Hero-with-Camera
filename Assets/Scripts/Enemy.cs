using TMPro;
using Unity.VisualScripting;
using UnityEngine;

// Finally added comments to everything 5/9/2025
// Class used to control enemy behavior
public class Enemy : MonoBehaviour
{
    // Initial health of the enemy
    public float health = 100f;
    // Store the sprite renderer to change color when hit
    public SpriteRenderer spriteRenderer;

    // Store number of enemies touched by player, total number of enemies, and number of enemies destroyed
    public static int TouchedEnemy = 0;
    public static int enemyCount = 0;
    public static int destroyed = 0;

    // Store the number of times the enemy has been hit
    private int hitCount;
    // Store the current color of the sprite to modify alpha color upon enemy hit 
    public Color spriteColor;
    // Used to store the waypoint manager to be used for waypoint movement
    public WayPointManager wayPointManager;
    // Store the current waypoint that the enemy is moving towards
    private int currentWaypoint = 0;

    // Store whether or not enemy is moving sequentially or randomly to waypoints
    public static bool sequential = true;
    // Store the current waypoint mode as a string
    public static string waypointMode = "Sequential";

    private void Awake()
    {
        // When instantiated hitcount is 0 and enemy count is incremented
        hitCount = 0;
        enemyCount++;
    }

    private void OnDestroy()
    {
        // When destroyed, enemy count is decremented and destroyed count is incremented
        enemyCount--;
        destroyed++;
    }


    void Start()
    {
        // Set the sprite renderer to the sprite renderer of the enemy
        spriteColor = spriteRenderer.color;
        wayPointManager = Object.FindFirstObjectByType<WayPointManager>();

        // Set first waypoint to a random waypoint that plane visits initially sequentially
        currentWaypoint = Random.Range(0, wayPointManager.waypoints.Length);
    }

    void Update()
    {
        // Call to move towards the current waypoint
        MoveTowardsWaypoint();
        // Set waypoint movement string to be used in UI
        if(sequential == true)
        {
            waypointMode = "Sequential";
        }
        else
        {
            waypointMode = "Random";
        }
    }

    // When the enemy collides with the player or a bullet, take damage or destroy the enemy
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            health = 0;
            Destroy(gameObject);
            TouchedEnemy++;
        }
        else if (other.CompareTag("Bullet"))
        {
            TakeDamage();
        }
    }

    // Function to take damage, reduce health, and change alpha color of the sprite
    public void TakeDamage()
    {
        // When the enemy is hit, reduce health by 20% and change alpha color of the sprite
        hitCount++;
        health *= 0.8f;
        if (hitCount == 4)
        {
            Destroy(gameObject);
        }
        // Change the alpha color of the sprite to indicate damage reduce alpha by 20% or 80% of previous alpha
        spriteColor.a *= 0.8f;
        spriteRenderer.color = spriteColor;
    }
    private void MoveTowardsWaypoint()
    {
        // Get the current waypoint position from the WayPointManager   
        Vector2 targetPosition = wayPointManager.GetCurrentWayPoint(currentWaypoint);

        // Calculate the direction to the target position
        Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;

        // Calculate the angle to rotate towards
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f; 
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);

        // Rotate towards the target rotation
        float rotationSpeed = 120f; 
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Move towards the target position
        float speed = 30f; 
        transform.position += transform.up * speed * Time.deltaTime;

        // If enemy is within 30 units move to next waypoint
        if (Vector2.Distance(transform.position, targetPosition) <= 30f)
        {
            // Sequentially move 
            if (sequential == true)
            {
                currentWaypoint++;
                if (currentWaypoint >= wayPointManager.waypoints.Length)
                {
                    currentWaypoint = 0;
                }
            }
            else
            {
                // Randomly move to a different waypoint
                int genIndex = Random.Range(0, wayPointManager.waypoints.Length);
                
                // While loop to ensure that the enemy does not return to the same waypoint
                while (genIndex == currentWaypoint)
                {
                    genIndex = Random.Range(0, wayPointManager.waypoints.Length);
                }
                currentWaypoint = genIndex;
            }
        }

    }
}
