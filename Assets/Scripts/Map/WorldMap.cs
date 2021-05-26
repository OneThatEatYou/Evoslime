using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "NewWorldMap", menuName = "ScriptableObjects/WorldMap")]
public class WorldMap : ScriptableObject
{
    [System.Serializable]
    public class SceneRow
    {
        public Scene[] sceneCols;
    }

    public SceneRow[] map;
}
