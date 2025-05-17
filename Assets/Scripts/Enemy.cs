using System.Collections;
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
    // Store the player object to be used for chases
    public Transform player;
    // Store whether or not the enemy is chasing the player
    private bool chasing = false;
    // Store whether or not enemy is touching player
    public bool isTouchingPlayer = false;
    // Store whether or not enemy is rotating due to touching player
    public bool isRotating = false;
    // Store whether or not enemy is rotating due to being hit twice
    private bool spinningFromHit;
    // Store the sprite to be used upon being hit twice from projectiles
    public Sprite secondHitSprite;

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

        player = GameObject.FindGameObjectWithTag("Player").transform;
        if (player != null)
        {
            player = player.transform;
        }
    }

    void Update()
    {
        // If currently rotating (from second hit) then do not move
        if (isRotating)
        {
            return;
        }
        // If number of projectiles hit is less than 2
        if (hitCount < 2)
        {
            // Check if enemy is chasing player
            if (chasing == true)
            {
                // If enemy is chasing player, set camera to focus on enemy
                EnemyManager.instance.FocusCameraOnChasingEnemy(this);
                float distance = Vector2.Distance(transform.position, player.position);
                // If distance between enemy and player is less than 40 units, move towards player
                if (distance <= 40f)
                {
                    MoveTowardsPlayer();
                }
                else
                {
                    chasing = false;
                }
            }
            else
            {
                // If enemy is not chasing player, set camera to stop focusing on enemy
                EnemyManager.instance.StopCameraFocusIfChaseEnded(this);
                MoveTowardsWaypoint();
            }
            // Set waypoint movement string to be used in UI
            if (sequential == true)
            {
                waypointMode = "Sequential";
            }
            else
            {
                waypointMode = "Random";
            }
        }

    }

    // When the enemy collides with the player or a bullet, take damage or destroy the enemy
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !spinningFromHit && hitCount < 2)
        {
            // Uncomment if want enemy to destroy on player collision
            // health = 0;
            // Destroy(gameObject);

            // If enemy collides with player increment number of enemies and set chase state to true
            Debug.Log("Touched Enemy");
            isTouchingPlayer = true;
            StartCoroutine(TouchPlayerRoutine());
            
            chasing = true;
            TouchedEnemy++;
        }
        else if (other.CompareTag("Bullet"))
        {
            // Store linear velocity of bullet to use to push enemy back
            // If enemy is hit a second time
            Vector2 bulletDirection = other.GetComponent<Rigidbody2D>().linearVelocity;
            TakeDamage(bulletDirection);
        }
    }

    // When enemy leaves the player set isTouchingPlayer to false
    // If player leaves and enemy not dead or rotating, then 
    // Start chasing the player
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isTouchingPlayer = false;
            if (!isRotating)
            {
                chasing = true;
                TouchedEnemy++;
            }
        }
    }

    // Coroutine used to rotate the enemy when touching the player
    private IEnumerator TouchPlayerRoutine()
    {
        float timer = 0f;
        isRotating = true;

        // First do counterclockwise rotation (0.3s)
        float ccwTime = 0.3f;
        float elapsed = 0f;
        while (elapsed < ccwTime)
        {
            transform.Rotate(0, 0, 360f * Time.deltaTime);
            elapsed += Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }

        // Then do clockwise rotation (0.3s)
        float cwTime = 0.3f;
        elapsed = 0f;
        while (elapsed < cwTime)
        {
            transform.Rotate(0, 0, -360f * Time.deltaTime);
            elapsed += Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }

        // After 0.6s, stop rotating
        isRotating = false;

        // If player is still touching after 0.6s, die. Otherwise, chase.
        if (isTouchingPlayer && timer >= 0.6f)
        {
            Destroy(gameObject);
        }
        else
        {
            chasing = true;
            TouchedEnemy++;
        }
    }

    // Coroutine used to spin the enemy when hit by a bullet
    // Enemy should spin forever until it is hit again or destroyed
    private IEnumerator SpinFromHit()
    {
        // Set spin variables to true
        spinningFromHit = true;
        isRotating = true;
        // Rotate the enemy while it is spinning
        while (spinningFromHit)
        {
            transform.Rotate(0, 0, 360f * Time.deltaTime);
            yield return null;
        }
        // After the enemy is hit again, stop spinning
        isRotating = false;
    }

    // Coroutine used to stop the enemy from moving after second hit
    private IEnumerator StopForce(float delay)
    {
        // Get the rigidbody of the enemy and set the initial velocity
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        float timer = 0f;
        Vector2 initVelocity = rb.linearVelocity * 2; // Const 2 to push enemy farther

        // Gradually reduce the velocity of the enemy to zero over the given delay
        while (timer < delay)
        {
            float temp = timer / delay;
            rb.linearVelocity = Vector2.Lerp(initVelocity, Vector2.zero, temp);
            timer += Time.deltaTime;
            yield return null;
        }
        // After the delay, set the velocity to zero and stop the enemy from moving
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
    }


    // Function to take damage, reduce health, and change alpha color of the sprite
    public void TakeDamage(Vector2 bulletVel)
    {
        // When the enemy is hit, reduce health by 20% and change alpha color of the sprite
        hitCount++;
        health *= 0.8f;
        // If hitcount is 1 start spinning from hit
        if (hitCount == 1)
        {
            chasing = false;
            EnemyManager.instance.StopCameraFocusIfChaseEnded(this);
            if (!spinningFromHit)
            {
                StartCoroutine(SpinFromHit());
            }
        }
        // If hitcount is 2, stop spinning change sprite and add force to the enemy
        else if (hitCount == 2)
        {
            Debug.Log("Bullet velocity: " + bulletVel);
            spinningFromHit = false;

            spriteRenderer.sprite = secondHitSprite;
            Rigidbody2D rb = GetComponent<Rigidbody2D>();
            rb.AddForce(bulletVel.normalized * 20f, ForceMode2D.Impulse);
            isRotating = true;

            StartCoroutine(StopForce(0.8f));
        }

        // Uncomment for older hitcount system
        // if (hitCount == 4)
        // {
        //     Destroy(gameObject);
        // }

        // New hitcount system on third hit kill enemy
        if (hitCount == 3)
        {
            Destroy(gameObject);
        }


        // Change the alpha color of the sprite to indicate damage reduce alpha by 20% or 80% of previous alpha
        spriteColor.a *= 0.8f;
        spriteRenderer.color = spriteColor;
    }
    // Function to move the enemy towards the player for chase
    private void MoveTowardsPlayer()
    {
        // Get position of player and direction to player
        Vector2 targetPosition = player.position;
        Vector2 direction = (targetPosition - (Vector2)transform.position);

        // Normalize the direction vector
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);

        // Rotate towards the target rotation
        float rotationSpeed = 120f;
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Move towards the target position based upon the speed and direction
        float speed = 30f;
        transform.position += transform.up * speed * Time.deltaTime;
    }
    // Function to move the enemy towards the waypoint
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
