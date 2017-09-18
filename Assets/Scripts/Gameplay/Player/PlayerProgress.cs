using System.Collections;
using System.Collections.Generic;

namespace KekeDreamLand {

    [System.Serializable]
    public class LevelProgress {
        public bool finished = false;
        public int feathersCollected = 0;
        public bool[] specialItemsFound;

        public LevelProgress()
        {
            finished = false;
            feathersCollected = 0;
            specialItemsFound = new bool[4];
        }

        public LevelProgress(int feathersCollected, bool[] specialItemsFound)
        {
            finished = true;
            this.feathersCollected = feathersCollected;
            this.specialItemsFound = specialItemsFound;
        }
    }

    [System.Serializable]
    public class PlayerProgress {

        // TODO Cumulate the time played on each level.
        // TODO death count.
        //public int timePlayed;

        public int currentWorldIndex;
        public int currentNodeIndex;

        // Dictionnary of all level progress.
        public Dictionary<string, LevelProgress> finishedLevels;

        public PlayerProgress()
        {
            currentWorldIndex = 0;
            currentNodeIndex = 0;

            finishedLevels = new Dictionary<string, LevelProgress>();
        }
    }
}
