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
    public struct GraphTransition
    {
        public InputDirection inputNeeded;
        public int targetNodeindex;
    }

    public enum InputDirection
    {
        UP, DOWN, LEFT, RIGHT
    }
}