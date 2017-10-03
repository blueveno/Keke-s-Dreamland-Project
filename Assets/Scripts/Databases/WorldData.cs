using UnityEngine;

namespace KekeDreamLand
{
    public class WorldData : ScriptableObject
    {
        public string worldname = "Default world name";

        public int sunflowerSeedNeeded;

        public Sprite background = null;

        public int sunFlowerCount;
    }
}