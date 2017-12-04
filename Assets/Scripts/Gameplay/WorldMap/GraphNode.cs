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
        public Direction inputNeeded = Direction.DOWN;
        public int targetNodeindex = -1;
        public Path path;
        [Tooltip("Let -1 if this path is an entrance from another node. Else put the exit index linked to the bread.")]
        public int exitIndex = 1;

        public GraphTransition() {

        }
    }
}