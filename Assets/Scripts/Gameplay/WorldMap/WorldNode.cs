using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KekeDreamLand
{
    [System.Serializable]
    public class WorldNode : GraphNode
    {
        [Header("World node informations :")]
        public int worldIndex;

        protected new void Awake()
        {
            base.Awake();
        }
    }
}