using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KekeDreamLand
{
    /// <summary>
    /// Level progress is the informations about the player progress of a level.
    /// </summary>
    [System.Serializable]
    public class WorldProgress
    {
        // important to unlock the secret level of a world.
        public int sunFlowerSeedCollected = 0;

        // Dictionnary of all level progress.
        public Dictionary<int, LevelProgress> finishedLevels;

        public WorldProgress()
        {
            finishedLevels = new Dictionary<int, LevelProgress>();
        }
    }
}