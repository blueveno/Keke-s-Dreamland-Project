using UnityEngine;

namespace KekeDreamLand
{
    public class LevelData : ScriptableObject
    {
        [Header("Level configuration :")]
        public string levelName = "Default name";
        
        public bool isSecretLevel = false;
        [Tooltip("Sunflower seed needed if this level is secret.")]
        public int seedNeededToUnlock = 0;
        
        [Tooltip("Number of exit in this level.")]
        public int exitCount;

        [Tooltip("Speedrun time goal")]
        public int timerTodo = 120;

        [Tooltip("Music played in this level.")]
        public AudioClip levelMusic;

        [Header("Items informations :")]
        public int totalFeathers = 0;

        [Tooltip("0:Key, 1:Raisin bread, 2:Chocolatine, 3:Sun flower seed.")]
        public bool[] itemsPresent = new bool[4];

        [Tooltip("Data of treasure to found in this level, let empty if not treasure to found")]
        public Treasure treasureToFound;
    }
}