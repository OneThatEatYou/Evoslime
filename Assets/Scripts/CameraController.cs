using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [HideInInspector] public Transform target;
    public Vector2 cameraMax;
    public Vector2 cameraMin;

    [Header("Debug")]
    public bool useCameraBounds = true;

    float Height { get { return 2f * cam.orthographicSize; } }
    float Width { get { return Height * cam.aspect; } }
    Vector2 MaxPos { get { return new Vector2(cameraMax.x - Width/2, cameraMax.y - Height/2); } }
    Vector2 MinPos { get { return new Vector2(cameraMin.x + Width/2, cameraMin.y + Height/2); } }

    Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void Update()
    {
        Vector3 targetPos = target.transform.position;

        if (useCameraBounds)
        {
            targetPos.x = Mathf.Clamp(targetPos.x, MinPos.x, MaxPos.x);
            targetPos.y = Mathf.Clamp(targetPos.y, MinPos.y, MaxPos.y);
        }
        
        targetPos.z = transform.position.z;
        transform.position = targetPos;
    }

    [ContextMenu("Print camera bounds")]
    void PrintCameraBounds()
    {
        Vector3 botLeft = new Vector3(cameraMin.x, cameraMin.y, 0);
        Vector3 botRight = new Vector3(cameraMax.x, cameraMin.y, 0);
        Vector3 topLeft = new Vector3(cameraMin.x, cameraMax.y, 0);
        Vector3 topRight = new Vector3(cameraMax.x, cameraMax.y, 0);

        Debug.Log($"TopLeft: {topLeft}");
        Debug.Log($"TopRight: {topRight}");
        Debug.Log($"BotLeft: {botLeft}");
        Debug.Log($"BotRight: {botRight}");
    }

    [ContextMenu("Print camera size")]
    void PrintCameraSize()
    {
        if (!cam)
        {
            cam = GetComponent<Camera>();
        }

        Debug.Log($"Height: {Height}, Width: {Width}");
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 botLeft = new Vector3(cameraMin.x, cameraMin.y, 0);
        Vector3 botRight = new Vector3(cameraMax.x, cameraMin.y, 0);
        Vector3 topLeft = new Vector3(cameraMin.x, cameraMax.y, 0);
        Vector3 topRight = new Vector3(cameraMax.x, cameraMax.y, 0);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, botRight);
        Gizmos.DrawLine(botRight, botLeft);
        Gizmos.DrawLine(botLeft, topLeft);
    }
}
