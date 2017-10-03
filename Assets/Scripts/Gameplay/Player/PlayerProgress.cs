using System.Collections;
using System.Collections.Generic;

namespace KekeDreamLand {
    
    /// <summary>
    /// player progress is the global informations about the game progress.
    /// </summary>
    [System.Serializable]
    public class PlayerProgress {

        // TODO cumulate the time played on each level in seconds.
        // public int timePlayed;

        // TODO death count.
        // public int deathCount;

        public int currentWorldIndex;
        public int currentNodeIndex;

        public WorldProgress[] worldProgress;
        private int worldCount = 1; // TODO update when add new world.

        // Treasures (key, found).
        public Dictionary<string, bool> treasuresFound;

        public PlayerProgress()
        {
            currentWorldIndex = 0;
            currentNodeIndex = 0;

            worldProgress = new WorldProgress[worldCount];
            for (int i = 0; i < worldCount; i++)
            {
                worldProgress[i] = new WorldProgress();
            }

            treasuresFound = new Dictionary<string, bool>();
        }
    }
}
