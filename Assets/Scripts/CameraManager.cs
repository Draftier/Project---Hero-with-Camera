using System.Collections;
using UnityEngine;

// Class used to control the camera manager and how it focuses on objects
public class CameraManager : MonoBehaviour
{
    // Store the object to be followed by the camera
    public GameObject ObjectToFollow;
    // Store the z offset for the camera
    public float zOffset = -10f;
    // Store whether or not the camera should be monitoring on start
    public bool monitorOnStart;
    // Store the camera object to be used for following the object
    private Camera cameraObject;
    // Store the background to be used if camera is inactive
    public GameObject background;

    private void Start()
    {
        // Set the camera to be orthographic and set the size
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        cameraObject = GetComponent<Camera>();

        // Set the minimap camera's viewport rect
        cameraObject.depth = 1;
        cameraObject.clearFlags = CameraClearFlags.Depth;
        cameraObject.orthographic = true;
        cameraObject.orthographicSize = 20f;

        Rect viewport = cameraObject.rect;

        // enable camera if on start true disable if false
        if (monitorOnStart == true && ObjectToFollow != null)
        {
            cameraObject.enabled = true;
        }
        else
        {
            cameraObject.enabled = false;
        }
    }

    // Called once per frame after everything else in update functions
    void LateUpdate()
    {
        // If objectToFollow not null and camera enabled
        // Follow objectToFollow
        if (ObjectToFollow != null && cameraObject.enabled)
        {
            Vector3 newPosition = ObjectToFollow.transform.position;
            newPosition.z = zOffset;
            transform.position = newPosition;
        }
    }

    // Method used to focus on a specific object
    public void FocusOnObject(GameObject obj)
    {   
        // Set object to follow
        ObjectToFollow = obj;

        // If objectToFollow not null and camera enabled
        // Enabled camerobject
        if (cameraObject != null && !cameraObject.enabled)
        {
            cameraObject.enabled = true;
        }
        
        // If background set
        if (background != null)
        {
            // Set background to false
            background.SetActive(false);
        }
    }

    // Method used to disable monitoring of the camera
    // and set the background to active
    public void DisableMonitoring()
    {
        // Set object to follow to null
        ObjectToFollow = null;
        
        // If camera object not null and enabled
        if (cameraObject != null && cameraObject.enabled)
        {
            // Disable camera object
            cameraObject.enabled = false;
        }
        // If background not null
        if (background != null)
        {
            // Set background to active
            background.SetActive(true);
        }
    }

    // Method used to focus on a static position (currently used for waypoints to show waypoint shake)
    public void FocusOnStaticPosition(Vector3 position)
    {
        // Set object to follow to null
        ObjectToFollow = null;

        // If camera object not null and enabled
        if (cameraObject != null && !cameraObject.enabled)
        {
            // Enable camera object
            cameraObject.enabled = true;
        }

        // Set the camera position to the given position
        transform.position = new Vector3(position.x, position.y, zOffset);

        // If background not null
        if (background != null)
        {
            // Set background to inactive
            background.SetActive(false);
        }
    }
}
