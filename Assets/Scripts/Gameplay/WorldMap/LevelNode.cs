using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KekeDreamLand
{
    [System.Serializable]
    public class LevelNode : GraphNode
    {
        [Header("Level node informations :")]
        public int worldIndex;
        public int levelIndex;

        public LevelData data;

        protected new void Awake()
        {
            base.Awake();

            // Indicates to the linked path that this level is secret.
            if (data.isSecretLevel)
                foreach (GraphTransition gt in linkedNodes)
                {
                    gt.path.secretLevel = true;
                }
        }
    }
}