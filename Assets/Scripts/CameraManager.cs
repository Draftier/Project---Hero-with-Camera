using System.Collections;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    // public RectTransform borderRect;
    public GameObject ObjectToFollow;
    public float zOffset = -10f;
    public bool monitorOnStart;
    private Camera cameraObject;
    public GameObject background;

    private void Start()
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        cameraObject = GetComponent<Camera>();
        // Set the minimap camera's viewport rect
        cameraObject.depth = 1;
        cameraObject.clearFlags = CameraClearFlags.Depth;
        cameraObject.orthographic = true;
        cameraObject.orthographicSize = 20f;

        Rect viewport = cameraObject.rect;

        float pixelX = viewport.x * screenWidth;
        float pixelY = viewport.y * screenHeight;
        float pixelWidth = viewport.width * screenWidth;
        float pixelHeight = viewport.height * screenHeight;

        // borderRect.anchorMin = new Vector2(0, 0);
        // borderRect.anchorMax = new Vector2(0, 0);
        // borderRect.pivot = new Vector2(0, 0);

        // borderRect.anchoredPosition = new Vector2(pixelX, pixelY);
        // borderRect.sizeDelta = new Vector2(pixelWidth, pixelHeight);

        if (monitorOnStart == true && ObjectToFollow != null)
        {
            cameraObject.enabled = true;
        }
        else
        {
            cameraObject.enabled = false;
        }
    }

    void LateUpdate()
    {
        if (ObjectToFollow != null && cameraObject.enabled)
        {
            Vector3 newPosition = ObjectToFollow.transform.position;
            newPosition.z = zOffset;
            transform.position = newPosition;
        }
    }

    public void FocusOnObject(GameObject obj)
    {
        ObjectToFollow = obj;
        if (cameraObject != null && !cameraObject.enabled)
        {
            cameraObject.enabled = true;
        }
        if (background != null)
        {
            Debug.Log("Background set to false");
            background.SetActive(false);
        }
    }

    public void DisableMonitoring()
    {
        ObjectToFollow = null;
        if (cameraObject != null && cameraObject.enabled)
        {
            cameraObject.enabled = false;
        }
        if (background != null)
        {
            background.SetActive(true);
        }
    }

    public void FocusOnStaticPosition(Vector3 position)
    {
        ObjectToFollow = null;
        if (cameraObject != null && !cameraObject.enabled)
        {
            cameraObject.enabled = true;
        }
        transform.position = new Vector3(position.x, position.y, zOffset);
        if (background != null)
        {
            Debug.Log("Background set to false");
            background.SetActive(false);
        }
    }
}
