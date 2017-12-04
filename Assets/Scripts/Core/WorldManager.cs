using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KekeDreamLand {

    public class WorldManager : MonoBehaviour {

        public WorldData data;

        /// <summary>
        /// Return the number of sun flower seeds required to unlock the secret level.
        /// </summary>
        /// <returns></returns>
        public int CountSunflowerSeedNeeded()
        {
            int sunflowerSeedNeeded = 0;

            GameObject[] nodes = GameObject.FindGameObjectsWithTag("Node");

            foreach (GameObject gn in nodes)
            {
                LevelNode ln = gn.GetComponent<LevelNode>();
                
                if (ln) {

                    // Don't count secret level seed.
                    if (ln.data.isSecretLevel)
                        continue;

                    // if sunflower seed is present in this level, increment the counter.
                    if (ln.data.itemsPresent[3])
                        sunflowerSeedNeeded++;
                }
            }

            return sunflowerSeedNeeded;
        }
    }

}