using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KekeDreamLand
{
    /// <summary>
    /// Manage a level. Setup, internal transition, level HUD.
    /// </summary>
    public class LevelManager : MonoBehaviour
    {
        #region Inspector Attributes

        [Header("Level intro")]
        
        public float levelIntroDuration = 3.0f;

        public LevelData data;

        #endregion

        #region Boing, HUD and Camera.

        // Boing
        private GameObject boing;
        private BoingManager boingScript;

        // UI
        private LevelHUDManager hudMgr;
        private LevelIntroManager levelIntroMgr;
        private LevelOutroManager levelOutroMgr;

        // Camera
        private CustomCamera2DFollow cameraFollow;

        // Internal transition attributes.
        private GameObject nextArea; // next area where Boing will go.
        private Vector3 nextPosition; // next position on this area where Boing will spawn.

        #endregion

        #region Level state attributes

        /// <summary>
        /// Return true if level is paused.
        /// </summary>
        public bool IsLevelPaused
        {
            get { return isLevelPaused; }

            set {
                isLevelPaused = value;
                
                Time.timeScale = isLevelPaused ? 0.0f : 1.0f;
                
                hudMgr.PauseGame(isLevelPaused);
            }
        }
        private bool isLevelPaused = false;

        /// <summary>
        /// Return true if the level is finished.
        /// </summary>
        public bool IsLevelFinished
        {
            get { return isLevelFinished; }
            set { isLevelFinished = value; }
        }
        private bool isLevelFinished = false;

        /// <summary>
        /// Return true if an internal transition is in progress.
        /// </summary>
        public bool IsInternalTransition
        {
            get { return isInternalTransition; }
        }
        private bool isInternalTransition = false;

        // True if level intro is currently displayed.
        private bool isDisplayLevelIntro = false;

        // True if level outro is currently displayed.
        public bool IsDisplayLevelOutro
        {
            get { return isDisplayLevelOutro; }
            set { isDisplayLevelOutro = value; }
        }
        private bool isDisplayLevelOutro = false;

        /// <summary>
        /// Return true if a transition is in process.
        /// </summary>
        public bool IsTransition
        {
            get { return isLevelFinished || isInternalTransition || isDisplayLevelIntro || isDisplayLevelOutro; }
        }

        #endregion

        #region Level content

        // Ennemies
        GameObject[] ennemies;

        // Items
        private int featherCount = 0;
        
        private bool[] specialItemPresent = new bool[4];
        private bool[] specialItemFound = new bool[4];

        /// <summary>
        /// Return number of feathers collected.
        /// </summary>
        public int FeatherPickedUp
        {
            get { return featherPickedUp; }

            set
            {
                featherPickedUp = value;
                RefreshFeatherCount();
            }
        }
        private int featherPickedUp = 0;

        // Checkpoints
        private Checkpoint lastCheckpoint = null;
        public Checkpoint LastCheckPoint
        {
            get { return lastCheckpoint; }
            set {
                lastCheckpoint = value;
                // TODO clear list of stored item of Boing and destroy them from level.
            }
        }

        #endregion

        #region Unity methods

        private void Awake()
        {
            SetupEnnemies();

            SetupLevel();
        }

        #endregion

        #region Setup methods

        private void SetupEnnemies()
        {
            EnableAllEnnemies(false);

            Debug.Log(ennemies.Length + " enemies on the level");
        }

        private void EnableAllEnnemies(bool enabled)
        {
            // Get all alive ennemies.
            ennemies = GameObject.FindGameObjectsWithTag("Enemy");
            
            foreach (GameObject enemy in ennemies)
            {
                foreach (AIBehaviour ai in enemy.GetComponents<AIBehaviour>())
                {
                    ai.enabled = enabled;
                }
            }
        }

        // Setup all components and variables of the gamemanager.
        private void SetupLevel()
        {
            // Recover Boing and camera.
            boing = GameObject.FindGameObjectWithTag("Player");
            boingScript = boing.GetComponent<BoingManager>();
            cameraFollow = Camera.main.GetComponent<CustomCamera2DFollow>();

            // Setup Ui.
            GameObject ui = GameObject.FindGameObjectWithTag("UI");
            if (ui)
            {
                hudMgr = ui.transform.Find("HUD").GetComponent<LevelHUDManager>();
                levelIntroMgr = ui.transform.Find("LevelIntro").GetComponent<LevelIntroManager>();
                levelOutroMgr = ui.transform.Find("LevelOutro").GetComponent<LevelOutroManager>();
            }

            // Retrieve informations about the level.
            featherCount = data.totalFeathers;
            specialItemPresent = data.itemsPresent;

            // Update level HUD and level outro.
            UpdateLifePoints(boingScript.maxLifePoints);
            hudMgr.SetupFeatherIndicators(featherCount);
            
            for (int i = 0; i < specialItemPresent.Length; i++)
            {
                hudMgr.SetupSpecificItem(i, specialItemPresent[i]);
                levelOutroMgr.SetupSpecificItem(i, specialItemPresent[i]);
            }
        }

        #endregion

        #region Level transitions methods

        /// <summary>
        /// Display level name at the start of the level. Or skip it.
        /// </summary>
        /// <param name="levelNumber"></param>
        /// <param name="skipIntro">Skip directly the level intro.</param>
        /// <returns></returns>
        public IEnumerator DisplayLevelIntro(string levelNumber, bool skipIntro)
        {
            // Skip intro.
            if (skipIntro)
            {
                StartLevel();
                yield break;
            }
            
            isDisplayLevelIntro = true;

            // Configurate level name and world/level number.
            levelIntroMgr.SetupLevelIntro(levelNumber, data.levelName);
            levelOutroMgr.SetupLevelName(levelNumber, data.levelName);
            levelIntroMgr.TriggerDisplay();

            yield return new WaitForSeconds(levelIntroDuration);

            isDisplayLevelIntro = false;

            StartLevel();
        }

        private void StartLevel()
        {
            levelIntroMgr.gameObject.SetActive(false);

            GameManager.instance.ActivateAnimator();

            EnableAllEnnemies(true);
        }

        /// <summary>
        /// Display level outro and automatic save.
        /// </summary>
        public void LevelOutro()
        {
            // Stop all ennemies.
            EnableAllEnnemies(false);

            // Update stats before display them.
            levelOutroMgr.UpdateFeatherPickedUp(featherPickedUp, featherCount);
            
            for (int i = 0; i < specialItemFound.Length; i++)
            {
                if (specialItemPresent[i])
                    levelOutroMgr.UpdateSpecialItems(i, specialItemFound[i]);
            }

            // And Update comment and boing special anim.
            levelOutroMgr.UpdateLevelOutro();
            
            levelOutroMgr.DisplayStepByStep();
            
            // Automatic save.
            GameManager.instance.SaveLevelProgress(featherPickedUp, specialItemFound);

            isDisplayLevelOutro = false; // user can switch to world map.
        }

        /// <summary>
        /// Display all components directly.
        /// </summary>
        public void SkipOutro()
        {
            levelOutroMgr.DisplayDirectly();

            isDisplayLevelOutro = false; // user can switch to world map.
        }

        /// <summary>
        /// Trigger an internal transition (Switch to an another area in the same scene).
        /// </summary>
        /// <param name="newArea">New Area to reach.</param>
        /// <param name="newPosition">New position of Boing in this area.</param>
        public void PrepareInternalTransition(GameObject newArea, Vector3 newPosition)
        {
            isInternalTransition = true;

            // Set Boing invulnerable when he use a door.
            boingScript.IsInvulnerable = true;

            nextArea = newArea;
            nextPosition = newPosition;
            nextPosition.z = boing.transform.position.z;

            GameManager.instance.TriggerFadeIn();
        }

        /// <summary>
        /// Move boing and camera to the prepared area and position.
        /// </summary>
        public void MoveBoingToNewArea()
        {
            // Move boing on the new area.
            boing.transform.position = nextPosition;
            cameraFollow.CurrentArea = nextArea.GetComponent<AreaEditor>();

            // Remove invulnerability when he has reached the new area.
            boingScript.IsInvulnerable = false;

            nextArea = null;
            isInternalTransition = false;
        }

        #endregion

        #region Items methods

        /// <summary>
        /// Indicates to the level that the specified special item has been picked up.
        /// </summary>
        /// <param name="specialItemIndex"></param>
        public void PickSpecialItem(int specialItemIndex)
        {
            specialItemFound[specialItemIndex] = true;

            hudMgr.UnlockSpecialItem(specialItemIndex);
        }

        /// <summary>
        /// Pick a croissant and heal Boing.
        /// </summary>
        public void PickCroissant()
        {
            boingScript.LifePoints++;
        }

        /// <summary>
        /// Return true if all feathers has been collected.
        /// </summary>
        /// <returns></returns>
        public bool HasCollectAllFeathers()
        {
            return featherPickedUp == featherCount;
        }

        /// <summary>
        /// Return true if all special items have been collected.
        /// </summary>
        /// <returns></returns>
        public bool HasCollectAllItems()
        {
            bool allFound = true;

            for (int i = 0; i < specialItemFound.Length && allFound; i++)
            {
                if (specialItemPresent[i])
                    allFound = specialItemFound[i];
            }

            return allFound;
        }

        #endregion

        #region HUD methods

        /// <summary>
        /// Request HUD to be toggled.
        /// </summary>
        public void ToggleHUD()
        {
            hudMgr.ToggleHUD();
        }

        /// <summary>
        /// Request lifepoints to be updated in hud.
        /// </summary>
        public void UpdateLifePoints(int lifePoints)
        {
            hudMgr.UpdateLifePoints(lifePoints);
        }

        private void RefreshFeatherCount()
        {
            hudMgr.UpdateFeatherPickedUp(featherPickedUp, featherCount);
        }

        #endregion

        #region Checkpoint methods

        /// <summary>
        /// Respawn Boing at the last checkpoint reached.
        /// </summary>
        public void RespawnAtCheckpoint()
        {
            // Determine area of the checkpoint.
            nextArea = lastCheckpoint.transform.parent.parent.parent.parent.gameObject;
            Transform parentInArea = nextArea.transform.Find("Level/Character");

            // Only one boing can exists.
            if(boing == null)
                boing = Instantiate(GameManager.instance.boingPrefab, parentInArea);

            // Set respawn position.
            Vector3 newPos = lastCheckpoint.gameObject.transform.position;
            newPos.z = 0.0f;
            
            // Move Boing and camera then fade out.
            nextPosition = newPos;
            MoveBoingToNewArea();

            GameManager.instance.TriggerFadeOut();
        }

        #endregion

        #region Utility methods

        // Utility methods for setup level data.

        // Count all feathers on the level and check for special items.
        public int CountFeathersInCurrentLevel()
        {
            GameObject[] feathers = GameObject.FindGameObjectsWithTag("Feather");
            return feathers.Length;
        }

        // Check all special items on the level.
        public bool[] CheckSpecialItemsPresent()
        {
            GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
            bool[] specialItems = new bool[4];

            foreach (GameObject item in items)
            {
                if (item.name.Contains("Key"))
                {
                    specialItems[0] = true;
                }

                else if (item.name.Contains("Sunflower seed"))
                {
                    specialItems[3] = true;
                }

                else if (item.name.Contains("Chocolatine"))
                {
                    specialItems[2] = true;
                }

            }

            // Raisin bread is available for all level. it is unlocked if the player collect all feathers.
            specialItems[1] = true;

            return specialItems;
        }

        #endregion
    }
}