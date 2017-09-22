using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KekeDreamLand
{
    /// <summary>
    /// Manage the world map : Load map and graph, navigation between nodes, display information about current node...
    /// </summary>
    public class WorldMapManager : MonoBehaviour
    {
        #region Inspector attributes

        public float boingSpeed = 10.0f;

        public Sprite[] worldMapSprites;
        public GameObject[] worldGraphPrefabs;

        public WorldMapHUDManager hudMgr;

        #endregion

        #region Private attributes

        /// <summary>
        /// Return the number of worlds on the game.
        /// </summary>
        public int WorldCount
        {
            get { return worldGraphPrefabs.Length; }
        }

        private SpriteRenderer currentBackground;

        // Current gameobject which store the graph.
        private GameObject currentGraph = null;

        // Current graph of the worldmap scene.
        private List<GraphNode> graph = new List<GraphNode>();
        
        private GameObject boing;

        private WorldData currentWorldData = null;

        #endregion
        
        #region Unity methods

        private void Awake()
        {
            boing = GameObject.FindGameObjectWithTag("Player");

            currentBackground = transform.Find("Background").gameObject.GetComponent<SpriteRenderer>();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Setup map depending of the player progress.
        /// </summary>
        /// <param name="playerProgress"></param>
        public void SetupMap(PlayerProgress playerProgress)
        {
            ChangeWorldMap(playerProgress.currentWorldIndex);

            SetupGraph();
            
            UnlockPaths(playerProgress);

            // Search current node where Boing is on the save.
            foreach (GraphNode g in graph)
            {
                if (g.nodeIndex == playerProgress.currentNodeIndex)
                {
                    // Place it on the world map without move along a path.
                    StartCoroutine(UpdateBoingPosition(null, g, playerProgress));
                }
            }
        }

        /// <summary>
        /// Try to move from the node to the specified direction.
        /// </summary>
        public void TryToMove(PlayerProgress playerProgress, Direction direction)
        {
            GraphNode node = graph.Find(x => x.nodeIndex == playerProgress.currentNodeIndex);

            // Check all paths.
            foreach (GraphTransition t in node.linkedNodes)
            {
                // Check if a direction is correct and the associated path is unlocked.
                if (t.inputNeeded == direction && t.path.unlocked)
                {
                    // Move along this path.
                    GraphNode targetNode = graph.Find(x => x.nodeIndex == t.targetNodeindex);

                    StartCoroutine(UpdateBoingPosition(t.path, targetNode, playerProgress));
                }
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

        #endregion

        #region Private methods

        private void SetupGraph()
        {
            // Remove all existing nodes.
            graph.Clear();

            GameObject[] nodes = GameObject.FindGameObjectsWithTag("Node");

            foreach (GameObject g in nodes)
            {
                GraphNode node = g.GetComponent<GraphNode>();
                if (node)
                    graph.Add(node);
            }
        }

        /// <summary>
        /// Unlock path depending the finished levels on the current world.
        /// O(n^2).
        /// </summary>
        /// <param name="playerProgress"></param>
        private void UnlockPaths(PlayerProgress playerProgress)
        {
            // Check all nodes of the world.
            foreach (GraphNode node in graph)
            {
                LevelNode ln = node as LevelNode;
                if (ln)
                {
                    LevelProgress levelProgress;

                    // Get informations about the player progress on this level.
                    if (playerProgress.worldProgress[ln.worldIndex].finishedLevels.TryGetValue(ln.levelIndex, out levelProgress))
                    {
                        // If found and level finished, unlock all paths of this node.
                        if (levelProgress.finished)
                            foreach (GraphTransition t in ln.linkedNodes)
                            {
                                if (!t.path.unlocked && !t.path.secretLevel)
                                {
                                    if (node.nodeIndex == playerProgress.currentNodeIndex)
                                        t.path.StartCoroutine(t.path.UnlockPath());
                                    else
                                        t.path.DisplayPath();
                                }

                                // Case of a path which is linked to a secret level node. 
                                else if(!t.path.unlocked && t.path.secretLevel)
                                {
                                    // Unlock path if all sunflower seeds have been collected.
                                    if(currentWorldData.sunflowerSeedNeeded == playerProgress.worldProgress[ln.worldIndex].sunFlowerSeedCollected)
                                        t.path.StartCoroutine(t.path.UnlockPath());
                                }
                            }
                    }
                }
            }
        }
        
        private IEnumerator UpdateBoingPosition(Path path, GraphNode targetNode, PlayerProgress playerProgress)
        {
            Vector2 targetPosition = targetNode.positionOnMap;

            // Move Boing along the path if not null.
            if (path != null)
            {
                // Reverse path if the target node is the start of the path.
                List<Vector2> pathPoints = path.waypoints;
                if (pathPoints[0] == targetPosition)
                    pathPoints.Reverse();

                // Move along the path.
                foreach (Vector2 t in pathPoints)
                {
                    targetPosition = t;
                    targetPosition.y += 0.75f;
                    
                    while(Vector2.Distance(boing.transform.position, targetPosition) >= Mathf.Epsilon)
                    {
                        boing.transform.position = Vector2.MoveTowards(boing.transform.position, targetPosition, boingSpeed * Time.deltaTime);
                        yield return null;
                    }

                    boing.transform.position = targetPosition;
                }
            }

            // Teleport Boing to the target point.
            else
            {
                targetPosition.y += 0.75f;
                boing.transform.position = targetPosition;
            }

            // Update worldmap HUD
            LevelNode ln = targetNode as LevelNode;
            WorldNode wn = targetNode as WorldNode;
            LevelProgress progress = null;
            LevelData levelData = null;

            // Check what is the new node.
            string whatIsIt = "";
            if (ln)
            {
                levelData = ln.data;

                if (levelData.isSecretLevel)
                    whatIsIt = "Secret level";
                else
                    whatIsIt = "Level " + (ln.worldIndex + 1) + "-" + (ln.levelIndex + 1);
                
                // Try to get progress.
                playerProgress.worldProgress[ln.worldIndex].finishedLevels.TryGetValue(ln.levelIndex, out progress);
            }
            else if (wn)

                whatIsIt = "Go to " + wn.worldDataTarget.worldname;
            
            hudMgr.UpdateLevelPreview(whatIsIt, levelData, progress);
            GameManager.instance.UpdateCurrentNodeOnWorld(targetNode.nodeIndex);
        }

        /// <summary>
        /// Change the current world map background and world map graph.
        /// </summary>
        /// <param name="worldIndex"></param>
        private void ChangeWorldMap(int worldIndex)
        {
            if (worldIndex >= worldMapSprites.Length || worldIndex >= worldGraphPrefabs.Length)
            {
                Debug.LogWarning("World " + (worldIndex + 1) + " doesn't exist !");
                return;
            }

            // Destroy the old graph if exists.
            if (currentGraph)
                Destroy(currentGraph);

            currentBackground.sprite = worldMapSprites[worldIndex];
            currentGraph = Instantiate(worldGraphPrefabs[worldIndex], transform);

            // Get data of this world.
            currentWorldData = currentGraph.GetComponent<WorldManager>().data;
        }

        private void SwitchToNewWorld(WorldNode node)
        {
            ChangeWorldMap(node.worldIndex);

            // TODO update current world index.
            // GameManager.instance.UpdateCurrentPosition(0);
        }

        private void SwitchToNewLevel(LevelNode node)
        {
            GameManager.instance.LoadNewLevel(node.worldIndex, node.levelIndex);
        }

        #endregion
    }

}
