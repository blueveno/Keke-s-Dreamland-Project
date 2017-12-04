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

        [Tooltip("Boing speed on world map")]
        public float boingSpeed = 10.0f;
        
        public GameObject[] worldGraphPrefabs;

        public WorldMapHUDManager hudMgr;

        public float offset = 0.05f;

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

        public bool IsTravelling { get; private set; }

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
        public IEnumerator SetupMap(PlayerProgress playerProgress)
        {
            ChangeWorldMap(playerProgress.currentWorldIndex);

            yield return new WaitForEndOfFrame();

            SetupGraph();
            
            UnlockPaths(playerProgress);

            GraphNode newNode = UpdateBoingPosition(playerProgress.currentNodeIndex);
            UpdateWorldMapHUD(newNode, playerProgress);
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
                StartCoroutine(SwitchToNewWorld(wn));
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
                if (ln == null)
                    continue;
                
                LevelProgress levelProgress;
                // If found informations about the player progress on this level, try to unlock.
                if (playerProgress.worldProgress[ln.worldIndex].finishedLevels.TryGetValue(ln.levelIndex, out levelProgress))
                {
                    // If found and level finished, 
                    if (!levelProgress.finished)
                        continue;

                    // Unlock all paths of this node.
                    foreach (GraphTransition t in ln.linkedNodes)
                    {
                        // Check if this path is an entrance.
                        if(t.exitIndex != -1)
                        {
                            // If not, check if this exit is unlocked.
                            if (!levelProgress.exits[t.exitIndex])
                                continue;
                        }

                        PathToSecret pathToSecret = t.path as PathToSecret;
                        if (pathToSecret == null)
                        {
                            if (!t.path.unlocked)
                            {
                                if (node.nodeIndex == playerProgress.currentNodeIndex)
                                    t.path.StartCoroutine(t.path.UnlockPath());

                                else
                                    t.path.DisplayPath();
                            }
                        }

                        else
                        {
                            // Retrieve data of the secret level node.
                            LevelNode secretLevelNode = graph.Find(x => x.nodeIndex == t.targetNodeindex) as LevelNode;
                            
                            // Get sunflower seed collected and needed
                            int sfsCollected = playerProgress.worldProgress[ln.worldIndex].sunFlowerSeedCollected;
                            int sfsNeeded = secretLevelNode.data.seedNeededToUnlock;

                            bool unlocked = sfsCollected >= sfsNeeded;

                            // Setup feedback only if necessary.
                            if (!unlocked)
                                pathToSecret.SetupUI(sfsCollected, sfsNeeded);
                            
                            // Display it progessively
                            if (node.nodeIndex == playerProgress.currentNodeIndex)
                                pathToSecret.StartCoroutine(pathToSecret.UnlockSecretPath(unlocked));

                            // Display it directly
                            else
                                pathToSecret.DisplaySecretPath(unlocked);
                        }
                    }
                }
            }
        }
        
        // Simpler change of boing position. Return the new node reached.
        private GraphNode UpdateBoingPosition(int targetNodeIndex)
        {
            // Search for the new node in the new graph.
            GraphNode node = graph.Find(x => x.nodeIndex == targetNodeIndex);

            // Find node position and adjust it to Boing.
            Vector2 targetPosition = node.positionOnMap;
            targetPosition.y += offset;

            boing.transform.position = targetPosition;

            return node;
        }

        private IEnumerator UpdateBoingPosition(Path path, GraphNode targetNode, PlayerProgress playerProgress)
        {
            Vector2 targetPosition = targetNode.positionOnMap;

            IsTravelling = true;

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
                    targetPosition.y += offset;
                    
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
                targetPosition.y += offset;
                boing.transform.position = targetPosition;
            }

            IsTravelling = false;

            UpdateWorldMapHUD(targetNode, playerProgress);
            GameManager.instance.UpdateCurrentNodeOnWorld(targetNode.nodeIndex);
        }

        private void UpdateWorldMapHUD(GraphNode targetNode, PlayerProgress playerProgress)
        {
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
        }

        /// <summary>
        /// Change the current world map background and world map graph.
        /// </summary>
        /// <param name="worldIndex"></param>
        private void ChangeWorldMap(int worldIndex)
        {
            if (worldIndex >= worldGraphPrefabs.Length)
            {
                Debug.LogWarning("World " + (worldIndex + 1) + " doesn't exist !");
                return;
            }

            // Destroy the old graph if exists.
            if (currentGraph)
                Destroy(currentGraph);
            
            currentGraph = Instantiate(worldGraphPrefabs[worldIndex], transform);

            // Get data of this world.
            currentWorldData = currentGraph.GetComponent<WorldManager>().data;

            // Update background.
            currentBackground.sprite = currentWorldData.background;
        }
        
        private IEnumerator SwitchToNewWorld(WorldNode worldNode)
        {
            GameManager.instance.TriggerFadeIn();

            yield return new WaitForSeconds(1.0f);
            
            // Notify gamemanager.
            GameManager.instance.MoveToNewWorld(worldNode.worldIndex, worldNode.targetNodeIndex);

            yield return new WaitForEndOfFrame();

            GameManager.instance.TriggerFadeOut();
        }

        private void SwitchToNewLevel(LevelNode node)
        {
            GameManager.instance.LoadNewLevel(node.worldIndex, node.levelIndex);
        }

        #endregion
    }

}
