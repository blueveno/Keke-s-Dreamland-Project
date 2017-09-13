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

        protected new void Awake()
        {
            base.Awake();
        }
    }
}