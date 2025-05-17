using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

// Class used to control waypoint behavior
public class WayPoint : MonoBehaviour
{
    // Store the number of waypoints spawned
    public static int wayPointCount = 0;
    // Store the waypoint manager to be used for spawning waypoints
    public WayPointManager wayPointManager;
    // Store the sprite renderer and color to modify alpha color upon waypoint hit
    public SpriteRenderer spriteRenderer;
    private Color spriteColor;
    // Store the number of times the waypoint has been hit
    private int hitCount;
    // Store the health of the waypoint
    public float Health = 100.0f;
    // Store the priority of the waypoint (set in editor for if I utilize prefab and manually want to set priority)
    public int priority;
    // Store whether or not the waypoint is shaking
    private bool shaking = false;

    void Awake()
    {
        // When instantiated reset hit count, alpha color, and increment waypoint count
        hitCount = 0;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteColor = spriteRenderer.color;
        wayPointCount++;
        wayPointManager = Object.FindFirstObjectByType<WayPointManager>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        // If waypoint hit by bullet take damage
        if (other.CompareTag("Bullet"))
        {
            TakeDamage();
        }
    }

    private void TakeDamage()
    {
        // When taking damage increment hit count, and set spritecolor to 80% of original alpha
        // And 75% of health before hit. If hitcount is 4 then reset hitcount, "spawn" a new waypoint, and reset health
        hitCount++;
        if (hitCount == 1)
        {
            // Set the camera to focus on the waypoint for 1 second
            wayPointManager.FocusOnWaypointForTime(gameObject, 1.0f);
            // Shake the waypoint for 1 second with a magnitude of 1
            ShakeWaypoint(1, 1);
        }
        else if (hitCount == 2)
        {
            // Set the camera to focus on the waypoint for 2 seconds
            wayPointManager.FocusOnWaypointForTime(gameObject, 2.0f);
            // Shake the waypoint for 2 seconds with a magnitude of 4
            ShakeWaypoint(2, 4);
        }
        else if (hitCount == 3)
        {
            // Set the camera to focus on the waypoint for 3 seconds
            wayPointManager.FocusOnWaypointForTime(gameObject, 3.0f);
            // Shake the waypoint for 3 seconds with a magnitude of 9
            ShakeWaypoint(3, 9);
        }
        else if (hitCount == 4)
        {
            // Doesn't actually destroy waypoint but sets it to new position
            wayPointManager.SpawnWayPoint(gameObject, gameObject.transform.position);
            hitCount = 0;
            Health = 100;
            spriteColor.a = 1.0f;
        }
        spriteColor.a *= 0.8f;
        Health *= 0.75f;
        spriteRenderer.color = spriteColor;
    }

    // Method used to shake the waypoint
    private void ShakeWaypoint(float duration, float magnitude)
    {
        // If the waypoint is not shaking, start shaking it
        if (shaking == false)
        {
            // Start shaking the waypoint for the given duration and magnitude
            StartCoroutine(ShakeNow(duration, magnitude));
            // Set the shaking flag to true
            shaking = true;
        }
        else if (shaking == true)
        {
            // If the waypoint is already shaking, stop the current shake coroutine and start a new one
            StopCoroutine(ShakeNow(duration, magnitude));
            StartCoroutine(ShakeNow(duration, magnitude));
        }
        // Set the shaking flag to false
        shaking = false;
    }

    // Coroutine used to shake the waypoint
    private IEnumerator ShakeNow(float duration, float magnitude)
    {
        // Store the original position of the waypoint
        Vector3 originalPos = transform.position;

        // Store the elapsed time
        float elapsed = 0.0f;

        // While the elapsed time is less than the duration
        while (elapsed < duration)
        {
            // Generate a random x and y position within the given magnitude
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            // Set the position of the waypoint to the original position plus the random x and y positions
            transform.position = new Vector3(x + originalPos.x, y + originalPos.y, originalPos.z);

            // Increment the elapsed time by the time since the last frame
            elapsed += Time.deltaTime;

            // Wait for the next frame
            yield return null;
        }
        // After the duration is over, set the position of the waypoint back to the original position
        transform.position = originalPos;
    }
}
