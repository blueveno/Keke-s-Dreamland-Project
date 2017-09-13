using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KekeDreamLand
{
    public class WorldMapManager : MonoBehaviour
    {
        public Sprite[] worldMapSprites;
        public GameObject[] worldGraphPrefabs;
                
        public int WorldCount
        {
            get { return worldGraphPrefabs.Length; }
        }

        private GameObject currentGraph = null;
        private List<GraphNode> graph = new List<GraphNode>();

        private GameObject boing;

        // World map = Graph - Nodes and transitions.

        // TODO load only world where the player is.

        // TODO change animator controller for another effect ?

        #region Unity methods

        private void Awake()
        {
            boing = GameObject.FindGameObjectWithTag("Player");
        }

        #endregion

        public void SetupMap(PlayerProgress playerProgress)
        {
            ChangeWorldMap(playerProgress.currentWorldIndex);

            SetupGraph();

            // TODO display all paths unlocked.
            UnlockPaths(playerProgress);

            // TODO Put Boing to the correct node of the graph.
            foreach (GraphNode g in graph)
            {
                if (g.nodeIndex == playerProgress.currentNodeIndex)
                {
                    UpdateBoingPosition(g);
                }
            }
        }

        // Temporary. TODO move animation along the path.
        private void UpdateBoingPosition(GraphNode g)
        {
            Vector2 boingPosition = g.positionOnMap;
            boingPosition.x -= 0.025f;
            boingPosition.y += 0.75f;

            boing.transform.position = boingPosition;
        }

        /// <summary>
        /// Change the current world map background and world map graph.
        /// </summary>
        /// <param name="worldIndex"></param>
        private void ChangeWorldMap(int worldIndex)
        {
            if (worldIndex >= worldMapSprites.Length || worldIndex >= worldGraphPrefabs.Length)
            {
                Debug.LogWarning("World " + (worldIndex+1) + " doesn't exist !");
                return;
            }

            // Destroy the old graph if exists.
            if (currentGraph)
                Destroy(currentGraph);

            GetComponent<SpriteRenderer>().sprite = worldMapSprites[worldIndex];
            currentGraph = Instantiate(worldGraphPrefabs[worldIndex], transform);
        }

        /// <summary>
        /// Unlock path depending the finished levels on the current world.
        /// </summary>
        /// <param name="playerProgress"></param>
        private void UnlockPaths(PlayerProgress playerProgress)
        {
            // TODO UnlockPaths
        }

        private void SetupGraph()
        {
            // Remove all existing nodes.
            graph.Clear();

            GameObject[] nodes = GameObject.FindGameObjectsWithTag("Node");

            foreach(GameObject g in nodes)
            {
                GraphNode node = g.GetComponent<GraphNode>();
                if (node)
                    graph.Add(node);
            }
        }

        /// <summary>
        /// Interact with the current node where Boing is.
        /// </summary>
        /// <param name="playerProgress">Informations about the player.</param>
        public void InteractWithCurrentNode(PlayerProgress playerProgress)
        {
            GraphNode node = graph.Find(x => x.nodeIndex == playerProgress.currentNodeIndex);

            LevelNode ln = node as LevelNode;
            WorldNode wn = node as WorldNode;

            if (ln)
                SwitchToNewLevel(ln);
            else if (wn)
                SwitchToNewWorld(wn);
        }

        /// <summary>
        /// 
        /// </summary>
        public void TryToMove(PlayerProgress playerProgress, InputDirection direction)
        {
            GraphNode node = graph.Find(x => x.nodeIndex == playerProgress.currentNodeIndex);

            foreach(GraphTransition t in node.linkedNodes)
            {
                if (t.inputNeeded == direction)
                {
                    GraphNode targetNode = graph.Find(x => x.nodeIndex == t.targetNodeindex);
                    UpdateBoingPosition(targetNode);
                    GameManager.instance.UpdateCurrentPosition(targetNode);
                }
            }
        }

        private void SwitchToNewWorld(WorldNode node)
        {
            ChangeWorldMap(node.worldIndex);
        }

        private void SwitchToNewLevel(LevelNode node)
        {
            GameManager.instance.LoadNewLevel(node.worldIndex, node.levelIndex);
        }


    }

}
