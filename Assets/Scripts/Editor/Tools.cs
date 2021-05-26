using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

public class Tools
{
    static EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;

    // Add a menu item with multiple levels of nesting
    [MenuItem("Tools/LoadMapScenes")]
    private static void LoadMapScenes()
    {
        Debug.Log($"Loading {scenes.Length} scenes");

        foreach (var scene in scenes)
        {
            Debug.Log($"Loading {scene.path}");
            EditorSceneManager.OpenScene(scene.path, OpenSceneMode.Additive);
        }
    }
}