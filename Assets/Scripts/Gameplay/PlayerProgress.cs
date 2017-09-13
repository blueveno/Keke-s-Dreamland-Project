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
    }

    public class PlayerProgress {

        // TODO Cumulate the time played on each level.
        //public int timePlayed;

        public int currentWorldIndex;
        public int currentNodeIndex;

        // First list is list of worlds that contains a list of bool to indicates that a specific level is finished.
        public List<List<LevelProgress>> finishedLevels;

        public PlayerProgress()
        {
            currentWorldIndex = 0;
            currentNodeIndex = 0;

            finishedLevels = new List<List<LevelProgress>>();
        }
    }
}
