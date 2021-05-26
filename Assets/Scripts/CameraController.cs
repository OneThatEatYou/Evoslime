using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [HideInInspector] public Transform target;
    public Vector2 mapSize;

    [Header("Debug")]
    public bool useCameraBounds = true;

    float Height { get { return 2f * cam.orthographicSize; } }
    float Width { get { return Height * cam.aspect; } }
    Vector2 MaxPos { get { return new Vector2(cameraMax.x - Width/2, cameraMax.y - Height/2); } }
    Vector2 MinPos { get { return new Vector2(cameraMin.x + Width/2, cameraMin.y + Height/2); } }
    MapManager MapManager { get { return GameManager.Instance.mapManager; } }

    Vector2 cameraMax;  // top right of camera viewport
    Vector2 cameraMin;  // bottom left of camera viewport

    Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void Start()
    {
        cameraMax = mapSize / 2 + MapManager.CurMapPos * mapSize;
        cameraMin = -mapSize / 2 + MapManager.CurMapPos * mapSize;

        PrintCameraBounds();
    }

    private void Update()
    {
        Vector3 targetPos = target.transform.position;

        if (targetPos.x > cameraMax.x || targetPos.y > cameraMax.y)
        {
            // target is above or right of border
            Vector2 dir = (Vector2)targetPos - cameraMax;
            MoveToNextSection(dir);
        }
        else if (targetPos.x < cameraMin.x || targetPos.y < cameraMin.y)
        {
            // target is below or left of border
            Vector2 dir = (Vector2)targetPos - cameraMax;
            MoveToNextSection(dir);
        }

        if (useCameraBounds)
        {
            targetPos.x = Mathf.Clamp(targetPos.x, MinPos.x, MaxPos.x);
            targetPos.y = Mathf.Clamp(targetPos.y, MinPos.y, MaxPos.y);
        }
        
        targetPos.z = transform.position.z;
        transform.position = targetPos;
    }

    // offset camera min and max viewport
    void MoveToNextSection(Vector2 dir)
    {
        Vector2Int dirInt = new Vector2Int(Mathf.CeilToInt(dir.x / mapSize.x), Mathf.CeilToInt(dir.y / mapSize.y));

        GameManager.Instance.mapManager.MoveToNewMap(dirInt);

        cameraMax += mapSize * dirInt;
        cameraMin += mapSize * dirInt;

        Debug.Log($"New cameraMax: {cameraMax}, New cameraMin: {cameraMin}");
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
        Vector3 botLeft;
        Vector3 botRight;
        Vector3 topLeft;
        Vector3 topRight;

        if (!Application.isPlaying)
        {
            // cameraMin and cameraMax is not set yet
            botLeft = new Vector3(-mapSize.x / 2, -mapSize.y / 2, 0) + transform.position;
            botRight = new Vector3(mapSize.x / 2, -mapSize.y / 2, 0) + transform.position;
            topLeft = new Vector3(-mapSize.x / 2, mapSize.y / 2, 0) + transform.position;
            topRight = new Vector3(mapSize.x / 2, mapSize.y / 2, 0) + transform.position;
        }
        else
        {
            botLeft = new Vector3(cameraMin.x, cameraMin.y, 0);
            botRight = new Vector3(cameraMax.x, cameraMin.y, 0);
            topLeft = new Vector3(cameraMin.x, cameraMax.y, 0);
            topRight = new Vector3(cameraMax.x, cameraMax.y, 0);
        }

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, botRight);
        Gizmos.DrawLine(botRight, botLeft);
        Gizmos.DrawLine(botLeft, topLeft);
    }
}
