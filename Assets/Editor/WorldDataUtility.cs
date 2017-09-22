using UnityEngine;
using UnityEditor;

using KekeDreamLand;

public class WorldDatautility
{
    [MenuItem("KekeDreamLand/WorldData/Create for this world")]
    public static void CreateAsset()
    {
        GameObject world = GetCurrentWorld();

        if (world == null)
        {
            Debug.LogWarning("No world loaded in the scene.");
            return;
        }

        ScriptableObjectUtility.CreateAsset<WorldData>("/Databases/Worlds", world.name);
    }
    
    [MenuItem("KekeDreamLand/WorldData/Update this world")]
    public static void UpdateLevelData()
    {
        GameObject world = GetCurrentWorld();

        if (world == null)
        {
            Debug.LogWarning("Place the world you want to update in Worldmap");
            return;
        }

        string worldName = world.name;

        Selection.activeObject = AssetDatabase.LoadMainAssetAtPath("Assets/Databases/Worlds/" + worldName + ".asset");
        WorldData worldData = Selection.activeObject as WorldData;

        // Check if level data for the current scene exists.
        if(worldData == null)
        {
            Debug.LogWarning("No world data found for this world. Create it ...");
            CreateAsset();
            return;
        }
        
        WorldManager worldMgr = world.GetComponent<WorldManager>();

        // TODO update sunflowerseed needed.
        worldData.sunflowerSeedNeeded = worldMgr.CountSunflowerSeedNeeded();

        // Make the asset savable.
        EditorUtility.SetDirty(worldData);

        worldMgr.data = worldData;
        // Make the level manager savable.
        EditorUtility.SetDirty(worldMgr);

        Debug.Log("The world data \"" + worldData.name + "\" has been correctly updated.\nPlease save the project. =)");
        }

    private static GameObject GetCurrentWorld()
    {
        GameObject worldMap = GameObject.Find("WorldMap");

        if (worldMap == null || worldMap.transform.childCount < 0)
            return null;

        GameObject world = null;

        foreach (Transform t in worldMap.transform)
        {
            if (t.name.Contains("World"))
                world = t.gameObject;
        }

        return world;
    }
}