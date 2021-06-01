using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class Tools
{
    [MenuItem("Tools/LoadMapScenes")]
    private static void LoadMapScenes()
    {
        WorldMap worldMap = Resources.Load<WorldMap>("BasicWorldMap");

        foreach (WorldMap.SceneRow sceneRow in worldMap.sceneRows)
        {
            foreach (string sceneName in sceneRow.sceneColNames)
            {
                Debug.Log($"Loading {sceneName}");
                EditorSceneManager.OpenScene($"Assets/Scenes/{sceneName}.unity", OpenSceneMode.Additive);
            }
        }
    }

    [MenuItem("Tools/SpawnOneShotAudio")]
    private static void SpawnOneShotAudioSource()
    {
        AudioManager.PlayAudioAtPosition(null, Vector2.zero, null, false);
    }
}