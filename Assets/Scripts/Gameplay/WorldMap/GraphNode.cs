using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KekeDreamLand
{
    [System.Serializable]
    public abstract class GraphNode : MonoBehaviour
    {
        [Header("Node informations :")]
        public int nodeIndex;
        public List<GraphTransition> linkedNodes; // max 4.

        [HideInInspector]
        public Vector2 positionOnMap;

        protected void Awake()
        {
            positionOnMap = transform.position;
        }
    }

    [System.Serializable]
    public class GraphTransition
    {
        public Direction inputNeeded;
        public int targetNodeindex;
        public Path path;

        public GraphTransition() {
            inputNeeded = Direction.DOWN;
            targetNodeindex = -1;
        }
    }
}