using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Rendering;

// Initially thought this would be used to generate random waypoints but I ended up using it 
// To define respawning of preplaced waypoints and to make sure they spawn within +-15units of 
// Previous position and within the screen bounds. Also can hide waypoints
public class WayPointManager : MonoBehaviour
{
    // Store the spawnrange for waypoints (set to +-15 for x and y in editor)
    public float spawnRangeX;
    public float spawnRangeY;
    // Store the screen bounds to be used for respawning waypoints
    private Vector3 screenBounds; 
    // Store the waypoints to be used for respawning
    public GameObject[] waypoints;
    // Store whether or not waypoints are hidden
    private bool isHidden = false;
    // Store waypoint visibility as a string
    public static string waypointVisibility = "visible";

    private void Awake()
    {
        // When instantiated set spawn bounds
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
    }
    // Method used to generate spawn position for waypoint based on previous position
    private Vector2 GenerateSpawnPosition(Vector2 previousPosition)
    {
        // Generate random spawn position within the screen bounds and within the spawn range (+-15 for x and y in editor)
        float spawnX = Random.Range(previousPosition.x-spawnRangeX, previousPosition.x + spawnRangeX);
        float spawnY = Random.Range(previousPosition.y-spawnRangeY, previousPosition.y +spawnRangeY);

        // Clamp the spawn position to be within the screen bounds
        spawnX = Mathf.Clamp(spawnX, -screenBounds.x, screenBounds.x);
        spawnY = Mathf.Clamp(spawnY, -screenBounds.y, screenBounds.y);
        return new Vector2(spawnX, spawnY);
    }

    // Method used to "spawn" a waypoint, since destroyed waypoints are not counted
    // Just move the waypoint when it gets "destroyed" to a new position 
    public void SpawnWayPoint(GameObject waypoint, Vector2 initialPosition)
    {
        Vector2 previousPosition = GenerateSpawnPosition(initialPosition);
        waypoint.transform.position = previousPosition;
    }

    // Method used to get the current waypoint based on given waypoint index
    public Vector2 GetCurrentWayPoint(int currentWaypointIndex)
    {
        // Get waypoints and sort them by priority
        List<GameObject> activeWaypoints = waypoints.Where(waypoint => waypoint != null)
        .OrderBy(waypoint => waypoint.GetComponent<WayPoint>().priority)
        .ToList();

        // If current waypoint index is less than the number of active waypoints, return the position of the current waypoint
        if(currentWaypointIndex < activeWaypoints.Count)
        {
            return activeWaypoints[currentWaypointIndex].transform.position;
        }
        else
        {
            // Otherwise return the position of first waypoint
            return activeWaypoints[0].transform.position;
        }
    }

    // Method used to hide all waypoints
    public void HideWayPoints()
    {
        isHidden = !isHidden;
        if(isHidden == true)
        {
            // Hides all waypoints by setting them to inactive
            waypointVisibility = "hidden";
            foreach (GameObject waypoint in waypoints)
            {
                if (waypoint != null)
                {
                waypoint.SetActive(false);
                }
            }
        }
        else if(isHidden == false)
        {
            // Reveals all waypoints by setting them to active
            waypointVisibility = "visible";
            foreach (GameObject waypoint in waypoints)
            {
                if (waypoint != null)
                {
                    waypoint.SetActive(true);
                }
            }
        }   
    }
}
