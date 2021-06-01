using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager
{
    public WorldMap worldMap;
    public Vector2 mapSize = new Vector2(48, 30);

    // +x is right, -x is left, +y is up, -y is down, (0, 0) is center
    public Vector2Int CurMapPos { get { return new Vector2Int(curMapCol - 1, 1 - curMapRow); } }
    bool isLoadingMap;
    public bool IsLoadingMap { get { return isLoadingMap; } }

    CameraController CamController { get { return GameObject.FindObjectOfType<CameraController>(); } }

    int curMapRow = 1;
    int curMapCol = 1;

    public MapManager()
    {
        LoadWorldMap();
    }

    void LoadWorldMap()
    {
        worldMap = Resources.Load<WorldMap>("BasicWorldMap");
    }

    public void MoveToNewMap(Vector2Int dirInt)
    {
        GameManager.Instance.StartCoroutine(LoadSection(dirInt));
    }

    IEnumerator LoadSection(Vector2Int dirInt)
    {
        isLoadingMap = true;

        string oldSceneName = worldMap.sceneRows[curMapRow].sceneColNames[curMapCol];
        string newSceneName = worldMap.sceneRows[curMapRow - dirInt.y].sceneColNames[curMapCol + dirInt.x];

        curMapRow -= dirInt.y;
        curMapCol += dirInt.x;

        AsyncOperation operation = SceneManager.LoadSceneAsync(newSceneName, LoadSceneMode.Additive);

        while (!operation.isDone)
        {
            yield return null;
        }

        if (dirInt.sqrMagnitude != 0)
        {
            // map did not move
            UnloadSection(oldSceneName);
        }

        CamController.MoveToNextSection(dirInt);

        isLoadingMap = false;
    }

    void UnloadSection(string sceneName)
    {
        SceneManager.UnloadSceneAsync(sceneName);
    }
}
