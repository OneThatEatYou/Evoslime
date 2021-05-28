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
    public EvolutionManager evolutionManager;

    private void Awake()
    {
        mapManager = new MapManager();
        evolutionManager = new EvolutionManager();
    }

    public void LoadScene(int sceneIndex, LoadSceneMode loadSceneMode)
    {
        SceneManager.LoadSceneAsync(sceneIndex, loadSceneMode);
    }

    public GameObject SpawnPlayer(string activeSceneName, MonsterData monsterData)
    {
        Scene activeScene = SceneManager.GetSceneByName(activeSceneName);

        if (!activeScene.IsValid())
        {
            Debug.LogWarning($"Invalid scene: {activeSceneName}");
            return null;
        }

        SceneManager.SetActiveScene(activeScene);

        GameObject player = Instantiate(monsterData.monsterPrefab, Vector2.zero, Quaternion.identity);
        player.AddComponent<PlayerController>();

        return player;
    }
}
