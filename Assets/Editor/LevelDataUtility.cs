using UnityEngine;
using UnityEditor;
using KekeDreamLand;

public class LevelDataUtility
{
    [MenuItem("Assets/Create/LevelData")]
    public static void CreateAsset()
    {
        ScriptableObjectUtility.CreateAsset<LevelData>("/Databases/Levels");
    }

    public static void UpdateLevelData()
    {
        // Update Level data selected with items present on the level and feathers...
    }
}