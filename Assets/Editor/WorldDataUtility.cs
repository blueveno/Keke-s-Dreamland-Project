using UnityEngine;
using UnityEditor;

using KekeDreamLand;

using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class WorldDatautility
{
    [MenuItem("KekeDreamLand/WorldData/Create for this world")]
    public static void CreateAsset()
    {
        string worldName = GetCurrentWorldName();

        if (worldName == null)
        {
            Debug.LogWarning("No world loaded in the scene.");
            return;
        } 

        ScriptableObjectUtility.CreateAsset<WorldData>("/Databases/Worlds", worldName);
    }
    
    [MenuItem("KekeDreamLand/WorldData/Update this world")]
    public static void UpdateLevelData()
    {
        string worldName = GetCurrentWorldName();

        if (worldName == null)
        {
            Debug.LogWarning("No world loaded in the scene.");
            return;
        }

        Selection.activeObject = AssetDatabase.LoadMainAssetAtPath("Assets/Databases/Worlds/" + worldName + ".asset");
        WorldData worldData = Selection.activeObject as WorldData;

        // Check if level data for the current scene exists.
        if(worldData == null)
        {
            Debug.LogWarning("No world data found for this world. Create it ...");
            CreateAsset();
            return;
        }

        // In any case : Assign the updated leveldata to the gamemanager.
        GameObject worldGameobject = GameObject.Find("WorldMap").transform.GetChild(0).gameObject;
        if (worldGameobject)
        {
            WorldManager worldMgr = worldGameobject.GetComponent<WorldManager>();

            // TODO update sunflowerseed needed.
            worldData.sunflowerSeedNeeded = worldMgr.CountSunflowerSeedNeeded();

            // Make the asset savable.
            EditorUtility.SetDirty(worldData);

            worldMgr.data = worldData;
            // Make the level manager savable.
            EditorUtility.SetDirty(worldMgr);

            Debug.Log("The world data \"" + worldData.name + "\" has been correctly updated.\nPlease save the project. =)");
        }
    }

    private static string GetCurrentWorldName()
    {
        GameObject worldMap = GameObject.Find("WorldMap");
        
        if (worldMap == null || worldMap.transform.childCount <= 0)
            return null;

        return worldMap.transform.GetChild(0).name;
    } 
}