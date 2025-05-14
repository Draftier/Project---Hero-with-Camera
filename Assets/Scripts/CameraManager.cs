using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public RectTransform borderRect;
    public GameObject ObjectToFollow;
    public float zOffset = -10f;

    private void Start()
    {
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        

        Camera cameraObject = GetComponent<Camera>();
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

        borderRect.anchorMin = new Vector2(0, 0);
        borderRect.anchorMax = new Vector2(0, 0);
        borderRect.pivot = new Vector2(0, 0);

        borderRect.anchoredPosition = new Vector2(pixelX, pixelY);
        borderRect.sizeDelta = new Vector2(pixelWidth, pixelHeight);
    }

    void LateUpdate()
    {
        if (ObjectToFollow != null)
        {
            Vector3 newPosition = ObjectToFollow.transform.position;
            newPosition.z = zOffset;
            transform.position = newPosition;
        }
    }
}
