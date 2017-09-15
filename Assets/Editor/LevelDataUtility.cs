using UnityEngine;
using UnityEditor;

using KekeDreamLand;

using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class LevelDataUtility
{
    [MenuItem("KekeDreamLand/LevelData/Create for this level")]
    public static void CreateAsset()
    {
        string currentSceneName = GetCurrentSceneName();

        ScriptableObjectUtility.CreateAsset<LevelData>("/Databases/Levels", currentSceneName);
    }
    
    [MenuItem("KekeDreamLand/LevelData/Update this level")]
    public static void UpdateLevelData()
    {
        string currentSceneName = GetCurrentSceneName();
        
        Selection.activeObject = AssetDatabase.LoadMainAssetAtPath("Assets/Databases/Levels/" + currentSceneName + ".asset");
        LevelData levelData = Selection.activeObject as LevelData;

        // Check if level data for the current scene exists.
        if(levelData == null)
        {
            Debug.LogWarning("No level data found for this level. Create it ...");
            CreateAsset();
            return;
        }

        // In any case : Assign the updated leveldata to the gamemanager.
        GameObject levelMgrGameobject = GameObject.Find("LevelManager");
        if (levelMgrGameobject)
        {
            LevelManager levelMgr = levelMgrGameobject.GetComponent<LevelManager>();

            levelData.totalFeathers = levelMgr.CountFeathersInCurrentLevel();
            levelData.itemsPresent = levelMgr.CheckSpecialItemsPresent();

            // A level is set secret if it contains a chocolatine.
            if (levelData.itemsPresent[2])
                levelData.isSecretLevel = true;

            // Make the asset savable.
            EditorUtility.SetDirty(levelData);

            levelMgr.data = levelData;
            // Make the level manager savable.
            EditorUtility.SetDirty(levelMgr);

            Debug.Log("The level data \"" + levelData.name + "\" has been correctly updated.\nPlease save the project. =)");
        }
    }

    private static string GetCurrentSceneName()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        
        if (!currentScene.IsValid())
            return null;

        return currentScene.name;
    } 
}