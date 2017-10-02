using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KekeDreamLand
{
    /// <summary>
    /// The GameManager permit to switch easily between scenes.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        #region Inspector attributes

        public GameObject boingPrefab;

        [Header("Build indexes :")]
        public int mainMenuIndex = 0;
        public int worldMapIndex = 1;

        #endregion

        #region Game Manager attributes

        /// <summary>
        /// Singleton of the gamemanager.
        /// </summary>
        public static GameManager instance = null;

        /// <summary>
        /// Level manager. Ready only.
        /// </summary>
        public LevelManager CurrentLevel { get; private set; }

        // Store the current level index to simplify save.
        private int currentLevelIndex;
        private int currentWorldIndex;

        // Animation transition script.
        private GameObject ui;
        private TransitionManager transitionManager;

        /// <summary>
        /// Return true if the current screen is the world map.
        /// </summary>
        public bool IsMainMenuScreen
        {
            get { return isMainMenu; }
        }
        private bool isMainMenu = false;
        private MainMenuManager mainMenu;

        /// <summary>
        /// Return true if the current screen is the world map.
        /// </summary>
        public bool IsWorldMapScreen
        {
            get { return isWorldMap; }
        }
        private bool isWorldMap = false;
        private WorldMapManager worldmap;

        // Load/Save system
        private PlayerProgress playerProgress;

        private int saveSlotSelected = 0;

        // Music manager.
        private MusicManager musicMgr;

        #endregion

        #region Unity methods

        private void Awake()
        {
            SingletonThis();
            musicMgr = GetComponent<MusicManager>();
        }

        private void OnEnable()
        {
            // Called only once because it's singleton.
            SceneManager.sceneLoaded += NewSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= NewSceneLoaded;
        }

        #endregion

        #region GameManager methods

        // Delegate method triggered when a new scene is loaded.
        private void NewSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            // Reset time scale if game was paused.
            Time.timeScale = 1.0f;

            // Reset information about the loaded scene.
            isMainMenu = false;
            mainMenu = null;

            isWorldMap = false;
            worldmap = null;

            CurrentLevel = null;

            // Setup transition manager of the current scene.
            ui = GameObject.FindGameObjectWithTag("UI");
            if (ui)
                transitionManager = ui.transform.Find("TransitionPanel").GetComponent<TransitionManager>();

            // Load main menu :
            if (arg0.buildIndex == mainMenuIndex)
            {
                // Here we can create new game or load existing game, we can also change settings...
                isMainMenu = true;
                
                mainMenu = GameObject.Find("MainMenuUI").GetComponent<MainMenuManager>();
                musicMgr.Play(mainMenu.titleMusic);
            }

            // Load world map :
            else if (arg0.buildIndex == worldMapIndex)
            {
                // Here we can move on the world map and enter in a level or access to an another world.

                isWorldMap = true;

                if (playerProgress != null)
                    currentWorldIndex = playerProgress.currentWorldIndex;

                // for debug only.
                else
                {
                    // Load first save.
                    playerProgress = SaveLoadManager.LoadPlayerProgress(saveSlotSelected);
                    currentWorldIndex = playerProgress.currentWorldIndex;
                }
                    
                worldmap = GameObject.Find("WorldMap").GetComponent<WorldMapManager>();
                StartCoroutine(worldmap.SetupMap(playerProgress));

                musicMgr.Play(worldmap.worldMapMusic);
            }

            // Load a level :
            else
            {
                // Try to get the level manager of this level.
                GameObject levelManager = GameObject.FindGameObjectWithTag("LevelManager");
                if (levelManager)
                {
                    CurrentLevel = levelManager.GetComponent<LevelManager>();
                    CurrentLevel.StartCoroutine(CurrentLevel.DisplayLevelIntro(arg0.name));

                    musicMgr.Play(CurrentLevel.data.levelMusic);
                }
            }
        }

        // Singleton this class.
        private void SingletonThis()
        {
            if (instance == null)
                instance = this;

            else
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        #endregion

        #region Load and Save System

        /// <summary>
        /// Create a new game in the specified slot. Display disclaimer if slot is not empty.
        /// </summary>
        /// <param name="selectedSlot"></param>
        /// <param name="disclaimer"></param>
        public void NewGame(int selectedSlot, bool disclaimer)
        {
            saveSlotSelected = selectedSlot;

            if (!disclaimer)
                NewGameThenStart();

            else
                StartCoroutine(ReplaceSave());
        }

        // TODO if usefull create delegate to generalize this method with string and success method.
        private IEnumerator ReplaceSave()
        {
            mainMenu.DisplayDisclaimer("Do you want to replace this save ?");

            yield return new WaitWhile(() => mainMenu.AnswerChoosen == -1);

            // Validate action.
            if(mainMenu.AnswerChoosen == 0)
                NewGameThenStart();

            // Prevent already answered for next disclaimer.
            else
                mainMenu.AnswerChoosen = -1;
        }

        private void NewGameThenStart()
        {
            // Create new progression.
            playerProgress = new PlayerProgress();

            // Create or replace old save.
            SaveLoadManager.SavePlayerProgress(playerProgress, saveSlotSelected);

            LoadWorldMap();
        }

        /// <summary>
        /// Store the current level progress and save it.
        /// </summary>
        /// <param name="feathersCollected"></param>
        /// <param name="itemsFound"></param>
        public void SaveLevelProgress(int feathersCollected, bool[] itemsFound, bool treasureFound)
        {
            // Debug.
            if (playerProgress == null)
                return;

            LevelProgress levelProgress = null;
            
            // Save found. Update for the best values.
            if (playerProgress.worldProgress[currentWorldIndex].finishedLevels.TryGetValue(currentLevelIndex, out levelProgress))
            {
                levelProgress.feathersCollected = Mathf.Max(feathersCollected, levelProgress.feathersCollected);
            }

            // level finished for the first time, create and add new entry.
            else
            {
                levelProgress = new LevelProgress(feathersCollected);

                playerProgress.worldProgress[currentWorldIndex].finishedLevels.Add(currentLevelIndex, levelProgress);
            }

            // Update items :
            for (int i = 0; i < levelProgress.specialItemsFound.Length; i++)
            {
                // Don't update item if already found.
                if (levelProgress.specialItemsFound[i])
                    continue;

                // Update if item has been found.
                if (itemsFound[i])
                {
                    levelProgress.specialItemsFound[i] = true;

                    // Sunflower seed obtained.
                    if (i == 3)
                        playerProgress.worldProgress[currentWorldIndex].sunFlowerSeedCollected++;
                }
            }

            // Update treasure if exist and hasn't already save.
            if (treasureFound && !levelProgress.treasureFound)
            {
                levelProgress.treasureFound = treasureFound;
                playerProgress.treasuresFound.Add(currentWorldIndex + "-" + currentLevelIndex, true);
            }

            // Save.
            SaveLoadManager.SavePlayerProgress(playerProgress, saveSlotSelected);
        }

        /// <summary>
        /// Load save slot selected.
        /// </summary>
        public void LoadPlayerProgress(int selectedSlot)
        {
            saveSlotSelected = selectedSlot;

            playerProgress = SaveLoadManager.LoadPlayerProgress(saveSlotSelected);

            LoadWorldMap();
        }

        #endregion

        #region Scene management

        /// <summary>
        /// Switch to the main menu scene.
        /// </summary>
        public void LoadMainMenu()
        {
            SceneManager.LoadScene(mainMenuIndex);
        }

        /// <summary>
        /// Switch to the world map scene.
        /// </summary>
        public void LoadWorldMap()
        {
            SceneManager.LoadScene(worldMapIndex);
        }

        /// <summary>
        /// Switch to the new specified level.
        /// </summary>
        /// <param name="world">world index</param>
        /// <param name="level">level index of this world</param>
        public void LoadNewLevel(int world, int level)
        {
            // Update current level index.
            currentLevelIndex = worldmap.GetLevelIndex(playerProgress.currentNodeIndex);

            SceneManager.LoadScene("Level " + (world + 1) + "-" + (level + 1));
        }

        /// <summary>
        /// Reset current loaded scene.
        /// </summary>
        public void ResetCurrentScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        #endregion

        #region Level management

        public void BoingDie()
        {
            TriggerFadeIn();
        }

        /// <summary>
        /// Use this when you want to finish the current level.
        /// </summary>
        public void FinishCurrentLevel()
        {
            CurrentLevel.IsDisplayLevelOutro = true;

            if(CurrentLevel.HasCollectAllFeathers())
            {
                Debug.Log("All feathers have been collected.");
                CurrentLevel.PickSpecialItem(1, true);
            }

            TriggerFadeIn();
        }

        #endregion

        #region World map management

        public void InteractWithCurrentNode()
        {
            worldmap.InteractWithCurrentNode(playerProgress);
        }

        public void MoveOnWorldMap(Direction directionPressed)
        {
            worldmap.TryToMove(playerProgress, directionPressed);
        }

        /// <summary>
        /// Update the current position of Boing on the current world.
        /// </summary>
        /// <param name="nodeIndex"></param>
        public void UpdateCurrentNodeOnWorld(int nodeIndex)
        {
            playerProgress.currentNodeIndex = nodeIndex;
        }

        /// <summary>
        /// Change Boing of world and update the position of Boing in this new world.
        /// </summary>
        /// <param name="worldIndex"></param>
        /// <param name="nodeIndex"></param>
        public void MoveToNewWorld(int worldIndex, int nodeIndex)
        {
            playerProgress.currentWorldIndex = worldIndex;
            playerProgress.currentNodeIndex = nodeIndex;

            // Setup new map.
            StartCoroutine(worldmap.SetupMap(playerProgress));
        }

        public bool CanMoveOnWorldMap()
        {
            return worldmap.IsTravelling;
        }

        #endregion

        #region MainMenu management

        public void GoToMainMenu()
        {
            mainMenu.SwitchTo(mainMenu.menuScreen);
        }

        public void BackInMenu()
        {
            mainMenu.Back();
        }

        /// <summary>
        /// Return true if the current screen is the title screen.
        /// </summary>
        /// <returns></returns>
        public bool IsTitleScreen()
        {
            return mainMenu.IsSpecifiedScreen(mainMenu.titleScreen);
        }

        /// <summary>
        /// Return true if the disclaimer is active.
        /// </summary>
        /// <returns></returns>
        public bool IsDisclaimer()
        {
            return mainMenu.disclaimer.activeSelf;
        }

        public void CancelDisclaimer()
        {
            mainMenu.ChooseDisclaimerAnswer(1);
        }

        #endregion

        #region Transition management

        /// <summary>
        /// Activate transition animator.
        /// </summary>
        public void ActivateAnimator()
        {
            transitionManager.ActivateAnimator();
        }

        /// <summary>
        /// Use this when you want to fade in and reload the current scene.
        /// </summary>
        public void TriggerFadeIn()
        {
            transitionManager.FadeIn();
        }

        public void TriggerFadeOut()
        {
            transitionManager.FadeOut();
        }

        // TODO use delegate to easily done what we want when fadeIn is finished ?

        /// <summary>
        /// Event triggered when a fadeIn transition animation finished.
        /// </summary>
        public void FadeInFinished()
        {
            // Case of a level scene.
            if(CurrentLevel)
            {
                // Case of an end of level.
                if (CurrentLevel.IsDisplayLevelOutro)
                {
                    CurrentLevel.LevelOutro();
                    return;
                }

                // Case of an internal transition.
                else if (CurrentLevel.IsInternalTransition)
                {
                    // Move Boing and Camera view when user don't see.
                    CurrentLevel.MoveBoingToNewArea();

                    transitionManager.FadeOut();
                    return;
                }

                // Case of a death of Boing.
                else
                {
                    // Respawn to checkpoint.
                    if (CurrentLevel.LastCheckPoint != null)
                    {

                        // Reactivate all items collected and mobs killed from the last checkpoint.
                        CurrentLevel.ReactivateAllItems();
                        CurrentLevel.ReactivateAllMobs();

                        // Respawn new Boing.
                        CurrentLevel.RespawnAtCheckpoint();
                    }

                    // Reset scene if no checkpoint in the level.
                    else
                    {
                        ResetCurrentScene(); // Obsolete. This case will probably never happens again.
                    }
                }
            }

            // Case of the world map.
            else if(isWorldMap)
            {
                // TODO FadeOut when change the world.
            }
            
            // Case of the main menu.
            else
            {
                LoadMainMenu();
            }
        }

        #endregion

        #region Music management

        /// <summary>
        /// Play the specified music.
        /// </summary>
        /// <param name="music"></param>
        public void PlayMusic(AudioClip music)
        {
            musicMgr.Play(music);
        }

        /// <summary>
        /// Update the volume of the music.
        /// </summary>
        /// <param name="exposedParameter"></param>
        /// <param name="newVolume"></param>
        public void SetVolume(string exposedParameter, float newVolume)
        {
            musicMgr.SetVolume(exposedParameter, newVolume);
        }

        #endregion
    }

}