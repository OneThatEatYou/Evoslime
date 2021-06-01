using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [HideInInspector] public Transform target;

    [Header("Debug")]
    public bool useCameraBounds = true;

    #region Properties
    float Height { get { return 2f * cam.orthographicSize; } }
    float Width { get { return Height * cam.aspect; } }
    Vector2 MaxPos { get { return new Vector2(cameraMax.x - Width/2, cameraMax.y - Height/2); } }
    Vector2 MinPos { get { return new Vector2(cameraMin.x + Width/2, cameraMin.y + Height/2); } }
    MapManager MapManager { get { return GameManager.Instance.mapManager; } }
    Vector2 MapSize { get { return MapManager.mapSize; } }
    #endregion

    Vector2 cameraMax;  // top right of camera viewport
    Vector2 cameraMin;  // bottom left of camera viewport

    Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void Start()
    {
        cameraMax = MapSize / 2 + MapManager.CurMapPos * MapSize;
        cameraMin = -MapSize / 2 + MapManager.CurMapPos * MapSize;

        MapManager.MoveToNewMap(new Vector2Int(0, 0));
    }

    private void Update()
    {
        if (target == null)
        { return; }

        Vector3 targetPos = target.transform.position;

        if ((targetPos.x > cameraMax.x || targetPos.y > cameraMax.y || targetPos.x < cameraMin.x || targetPos.y < cameraMin.y) && !MapManager.IsLoadingMap)
        {
            // target is out of camera view
            Vector2 dir = (Vector2)targetPos - MapManager.CurMapPos * MapSize;
            Vector2Int dirInt = new Vector2Int(Mathf.RoundToInt(dir.x / MapSize.x), Mathf.RoundToInt(dir.y / MapSize.y));

            GameManager.Instance.mapManager.MoveToNewMap(dirInt);
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
    public void MoveToNextSection(Vector2Int dirInt)
    {
        cameraMax += MapSize * dirInt;
        cameraMin += MapSize * dirInt;
    }

    #region Debug
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
    #endregion

    private void OnDrawGizmosSelected()
    {
        Vector3 botLeft;
        Vector3 botRight;
        Vector3 topLeft;
        Vector3 topRight;

        if (!Application.isPlaying)
        {
            // cameraMin and cameraMax is not set yet
            botLeft = new Vector3(-MapSize.x / 2, -MapSize.y / 2, 0) + transform.position;
            botRight = new Vector3(MapSize.x / 2, -MapSize.y / 2, 0) + transform.position;
            topLeft = new Vector3(-MapSize.x / 2, MapSize.y / 2, 0) + transform.position;
            topRight = new Vector3(MapSize.x / 2, MapSize.y / 2, 0) + transform.position;
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
