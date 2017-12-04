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
        }
    }
}