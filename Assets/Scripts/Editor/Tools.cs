using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class Tools
{
    const string Path = "Assets/Scenes/";
    const string SceneSuffix = ".unity";

    [MenuItem("Tools/LoadMapScenes")]
    private static void LoadMapScenes()
    {
        WorldMap worldMap = Resources.Load<WorldMap>("BasicWorldMap");

        foreach (WorldMap.SceneRow sceneRow in worldMap.sceneRows)
        {
            foreach (string sceneName in sceneRow.sceneColNames)
            {
                Debug.Log($"Loading {sceneName}");
                EditorSceneManager.OpenScene($"{Path}{sceneName}{SceneSuffix}", OpenSceneMode.Additive);
            }
        }
    }

    [MenuItem("Tools/SpawnOneShotAudio")]
    private static void SpawnOneShotAudioSource()
    {
        AudioManager.PlayAudioAtPosition(null, Vector2.zero, null, false);
    }

    [MenuItem("Tools/QuickLoadScene/MainMenu")]
    public static void QuickLoadMainMenu()
    {
        EditorSceneManager.OpenScene($"{Path}{GameManager.mainMenuSceneName}{SceneSuffix}", OpenSceneMode.Single);
    }

    [MenuItem("Tools/QuickLoadScene/GlobalScene")]
    public static void QuickLoadGlobalScene()
    {
        EditorSceneManager.OpenScene($"{Path}{GameManager.mainSceneName}{SceneSuffix}", OpenSceneMode.Single);
    }
}