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
            wayPointManager.FocusOnWaypointForTime(gameObject, 1.0f);
            ShakeWaypoint(1, 1);
        }
        else if (hitCount == 2)
        {
            wayPointManager.FocusOnWaypointForTime(gameObject, 2.0f);
            ShakeWaypoint(2, 4);
        }
        else if (hitCount == 3)
        {
            wayPointManager.FocusOnWaypointForTime(gameObject, 3.0f);
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

    private void ShakeWaypoint(float duration, float magnitude)
    {
        if(shaking == false)
        {
            StartCoroutine(ShakeNow(duration, magnitude));
            shaking = true;
        }
        else if(shaking == true)
        {
            StopCoroutine(ShakeNow(duration, magnitude));
            StartCoroutine(ShakeNow(duration, magnitude));
        }
        shaking = false;
    }

    private IEnumerator ShakeNow(float duration, float magnitude)
    {
        Vector3 originalPos = transform.position;

        float elapsed = 0.0f;

        while(elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            transform.position = new Vector3(x + originalPos.x, y + originalPos.y, originalPos.z);

            elapsed += Time.deltaTime;
            
            yield return null;
        }

        transform.position = originalPos;
    }
}
