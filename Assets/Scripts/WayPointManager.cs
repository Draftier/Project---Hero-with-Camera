using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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
    // Store the CameraManger to be used for focusing on waypoints
    public CameraManager cameraManager;
    // Store focus coroutine to be used for focusing on waypoints
    private Coroutine focusCoroutine;
    // Store whether or not waypoint is being focused on in text
    public TextMeshProUGUI waypointText;

    // Called before start function
    private void Awake()
    {
        // When instantiated set spawn bounds of waypoints
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
    }
    // Method used to generate spawn position for waypoint based on previous position
    private Vector2 GenerateSpawnPosition(Vector2 previousPosition)
    {
        // Generate random spawn position within the screen bounds and within the spawn range (+-15 for x and y in editor)
        float spawnX = Random.Range(previousPosition.x - spawnRangeX, previousPosition.x + spawnRangeX);
        float spawnY = Random.Range(previousPosition.y - spawnRangeY, previousPosition.y + spawnRangeY);

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
        if (currentWaypointIndex < activeWaypoints.Count)
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
        if (isHidden == true)
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
        else if (isHidden == false)
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
    
    // Call this method when a waypoint is hit
    public void FocusOnWaypointForTime(GameObject waypoint, float time)
    {
        if (focusCoroutine != null)
        {
            StopCoroutine(focusCoroutine);
        }
        Vector3 staticPosition = waypoint.transform.position;
        focusCoroutine = StartCoroutine(FocusRoutine(staticPosition, time));
    }

    // Coroutine used to focus on a waypoint for a given amount of time
    private IEnumerator FocusRoutine(Vector3 focusPosition, float time)
    {
        // If camera manager not null then focus on waypoint
        // And set waypoint cam text to active
        if (cameraManager != null)
        {
            waypointText.text = "WayPoint Cam: Active";
            cameraManager.FocusOnStaticPosition(focusPosition);
        }

        // Wait for the given amount of time
        yield return new WaitForSeconds(time);

        // If camera manager not null then set waypoint cam text to inactive
        // And disable monitoring
        if (cameraManager != null)
        {
            waypointText.text = "WayPoint Cam: Shut Off";
            cameraManager.DisableMonitoring();
        }

    }
}
