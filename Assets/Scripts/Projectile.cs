using UnityEngine;

// Class used to control projectile behavior
public class Projectile : MonoBehaviour
{
    // Store the screen bounds to be used for destroying projectiles when they leave the screen
    private Vector3 screenBounds;
    // Store number of projectiles currently not destroyed (on screen)
    public static int projectileCount = 0;
    // Store the speed of the projectile
    private float speed = 40.0f; 
    // Store the last position of the projectile to calculate speed
    Vector3 lastPosition;
    private void Awake()
    {
        // When instantiated, projectile count is incremented
        projectileCount++;
    }

    private void OnDestroy()
    {
        // When destroyed, projectile count is decremented
        projectileCount--;
    }
    void Start()
    {
        // Initialize the screen bounds and last position of the projectile
        lastPosition = transform.position;
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
    }

    void Update()
    {
        // Move the projectile "up" at a constant speed
        transform.Translate(Vector3.up * speed * Time.deltaTime);

        // Calculate speed of projectile
        float distance = Vector3.Distance(transform.position, lastPosition);
        Debug.Log("Speed this frame: " + distance / Time.deltaTime);
        lastPosition = transform. position;

        if(transform.position.y > screenBounds.y || transform.position.y < -screenBounds.y 
        || transform.position.x > screenBounds.x || transform.position.x < -screenBounds.x)
        {
            // Destroy projectile if it leaves screen bounds
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // If projectile hits enemy or waypoint destroy the projectile
        if(other.CompareTag("Enemy") || other.CompareTag("WayPoint"))
        {
            Destroy(gameObject);
        }
    }
}
