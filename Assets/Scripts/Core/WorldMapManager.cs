using System.Collections.Generic;
using UnityEngine;

namespace KekeDreamLand
{
    public class WorldMapManager : MonoBehaviour
    {
        #region Inspector attributes

        public Sprite[] worldMapSprites;
        public GameObject[] worldGraphPrefabs;

        #endregion

        #region Private attributes

        private GameObject currentGraph = null;

        private List<GraphNode> graph = new List<GraphNode>();
        
        private GameObject boing;
        
        /// <summary>
        /// Return the number of worlds on the game.
        /// </summary>
        public int WorldCount
        {
            get { return worldGraphPrefabs.Length; }
        }

        #endregion

        // World map = Graph - Nodes and transitions.

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
            
            // TODO animation when unlock ?
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
        /// O(n^3).
        /// </summary>
        /// <param name="playerProgress"></param>
        private void UnlockPaths(PlayerProgress playerProgress)
        {
            // Check all nodes of the world.
            foreach(GraphNode node in graph)
            {
                LevelNode ln = node as LevelNode;
                if (ln)
                {
                    LevelProgress levelProgress;
                    string key = ln.worldIndex + "-" + ln.levelIndex;

                    // Get informations about the player progress on this level.
                    if (playerProgress.finishedLevels.TryGetValue(key, out levelProgress))
                    {
                        // If found and level finished, unlock all paths of this node.
                        if (levelProgress.finished)
                            foreach(Transform t in node.gameObject.transform)
                            {
                                t.gameObject.SetActive(true);
                            }

                            // And unlock path to back to this node for each new unlocked nodes.
                            foreach (GraphTransition t in ln.linkedNodes)
                            {
                                if (!t.unlocked)
                                {
                                    t.unlocked = true;

                                    GraphNode targetNode = graph.Find(x => x.nodeIndex == t.targetNodeindex);
                                    foreach (GraphTransition t2 in targetNode.linkedNodes)
                                    {
                                        if (t2.targetNodeindex == ln.nodeIndex)
                                            t2.unlocked = true;
                                    }
                                }
                            }
                    }
                }
            }
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
        /// Try to move from the node to the specified direction.
        /// </summary>
        public void TryToMove(PlayerProgress playerProgress, InputDirection direction)
        {
            GraphNode node = graph.Find(x => x.nodeIndex == playerProgress.currentNodeIndex);
            
            // Check if a direction is correct.
            foreach (GraphTransition t in node.linkedNodes)
            {
                if (t.inputNeeded == direction && t.unlocked)
                {
                    GraphNode targetNode = graph.Find(x => x.nodeIndex == t.targetNodeindex);
                    UpdateBoingPosition(targetNode);
                    GameManager.instance.UpdateCurrentNodeOnWorld(t.targetNodeindex);
                }
            }
        }

        private void SwitchToNewWorld(WorldNode node)
        {
            ChangeWorldMap(node.worldIndex);

            // TODO update current world index.
            //GameManager.instance.UpdateCurrentPosition(0);
        }

        private void SwitchToNewLevel(LevelNode node)
        {
            GameManager.instance.LoadNewLevel(node.worldIndex, node.levelIndex);
        }

        /// <summary>
        /// Get level index associated to the node index.
        /// </summary>
        /// <param name="nodeIndex"></param>
        /// <returns></returns>
        public int GetLevelIndex(int nodeIndex)
        {
            int levelIndex = -1;

            GraphNode node = graph.Find(x => x.nodeIndex == nodeIndex);
            LevelNode ln = node as LevelNode;

            if (ln)
                levelIndex = ln.levelIndex;

            return levelIndex;
        }
    }

}
