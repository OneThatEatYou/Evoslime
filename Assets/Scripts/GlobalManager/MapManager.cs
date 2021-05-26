using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager
{
    public WorldMap worldMap;

    // x is left, -x is right, +y is up, -y is down, (0, 0) is center
    public Vector2Int CurMapPos { get { return new Vector2Int(curMapCol - 1, 1 - curMapRow); } }

    int curMapRow = 0;
    int curMapCol = 0;

    public MapManager()
    {
        LoadMap();
    }

    void LoadMap()
    {
        Debug.Log("Loading map");
        worldMap = Resources.Load<WorldMap>("BasicWorldMap");
    }

    public void MoveToNewMap(Vector2Int dir)
    {
        curMapRow -= dir.y;
        curMapCol += dir.x;

        LoadSection(worldMap.sceneRows[curMapRow].sceneColNames[curMapCol]);

        Debug.Log($"Moving to map ({curMapRow}, {curMapCol})");
    }

    void LoadSection(string sceneName)
    {
        Debug.Log($"Loading {sceneName}");
        SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
    }
}
