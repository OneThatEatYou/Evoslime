using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Singleton
    static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                //check if instance is in scene
                instance = FindObjectOfType<GameManager>();

                if (instance == null)
                {
                    //spawn instance
                    GameObject obj = new GameObject("GameManager");
                    instance = obj.AddComponent<GameManager>();
                    DontDestroyOnLoad(obj);
                }
            }
            return instance;
        }
    }
    #endregion

    public MapManager mapManager;

    private void Awake()
    {
        mapManager = new MapManager();
    }

    public void LoadScene(int sceneIndex, LoadSceneMode loadSceneMode)
    {
        SceneManager.LoadSceneAsync(sceneIndex, loadSceneMode);
    }
}
