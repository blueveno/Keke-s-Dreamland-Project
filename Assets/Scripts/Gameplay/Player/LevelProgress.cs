using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KekeDreamLand
{
    /// <summary>
    /// Level progress is the informations about the player progress of a level.
    /// </summary>
    [System.Serializable]
    public class LevelProgress
    {
        public bool finished = false;

        public int feathersCollected = 0;

        public bool[] specialItemsFound;

        public bool treasureFound = false;

        // TODO add best time for a level.

        public LevelProgress()
        {
            feathersCollected = 0;
            specialItemsFound = new bool[4];
        }

        public LevelProgress(int feathersCollected, bool[] specialItemsFound)
        {
            finished = true;
            this.feathersCollected = feathersCollected;
            specialItemsFound = new bool[4];
        }
    }
}