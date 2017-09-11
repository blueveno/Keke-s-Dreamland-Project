using System.Collections;
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

        public string levelName;
        public float levelIntroDuration = 3.0f;

        #endregion

        #region LevelManager Attributes

        // Boing
        private GameObject boing;
        private BoingManager boingScript;

        // UI
        private HUDManager hudMgr;
        private LevelIntroManager levelIntroMgr;
        private LevelOutroManager levelOutroMgr;

        // Camera
        private CustomCamera2DFollow cameraFollow;

        // internal transition attributes.
        private GameObject nextArea; // next area where Boing will go.
        private Vector3 nextPosition; // next position on this area where Boing will spawn.

        // Game manager attributes.

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

        public bool IsTransition
        {
            get { return isLevelFinished || isInternalTransition || isDisplayLevelIntro; }
        }

        // Items

        /// <summary>
        /// Return number of feathers collected.
        /// </summary>
        public int FeatherPickedUp
        {
            get { return featherPickedUp; }

            set {
                featherPickedUp = value;
                RefreshFeatherCount();
            }
        }
        private int featherPickedUp = 0;
        private int featherCount = 0;
        
        private bool[] specialItemPresent = new bool[4];
        private bool[] specialItemFound = new bool[4];

        // True if level intro is currently displayed.
        private bool isDisplayLevelIntro = false;

        #endregion

        #region Unity methods
        
        private void Awake()
        {
            SetupLevel();
        }

        #endregion

        #region Private methods

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
                hudMgr = ui.transform.Find("HUD").GetComponent<HUDManager>();
                levelIntroMgr = ui.transform.Find("LevelIntro").GetComponent<LevelIntroManager>();
                levelOutroMgr = ui.transform.Find("LevelOutro").GetComponent<LevelOutroManager>();
            }

            CountFeathersInCurrentLevel();

            // Special items.
            for (int i = 0; i < specialItemPresent.Length; i++)
            {
                specialItemPresent[i] = false;
            }
            CheckSpecialItemsPresent();

            // Update level HUD.
            UpdateLifePoints(boingScript.maxLifePoints);
            hudMgr.SetupFeatherIndicators(featherCount);

            for (int i = 0; i < specialItemPresent.Length; i++)
            {
                hudMgr.SetupSpecificItem(i, specialItemPresent[i]);
            }
        }

        // Count all feathers on the level and check for special items.
        private void CountFeathersInCurrentLevel()
        {
            GameObject[] feathers = GameObject.FindGameObjectsWithTag("Feather");
            featherCount = feathers.Length;
        }

        // Check all special items on the level.
        private void CheckSpecialItemsPresent()
        {
            GameObject[] items = GameObject.FindGameObjectsWithTag("Item");
            foreach (GameObject item in items)
            {
                if (item.name.Contains("Key"))
                {
                    specialItemPresent[0] = true;
                }

                else if (item.name.Contains("Sunflower seed"))
                {
                    specialItemPresent[3] = true;
                }

                else if (item.name.Contains("Chocolatine"))
                {
                    specialItemPresent[2] = true;
                }
                
            }

            // In all levels.
            specialItemPresent[1] = true;
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
                levelIntroMgr.gameObject.SetActive(false);
                GameManager.instance.ActivateAnimator();
                yield break;
            }
            
            isDisplayLevelIntro = true;

            // Configurate level name and world/level number.
            levelIntroMgr.SetupLevelIntro(levelNumber, levelName);
            levelIntroMgr.TriggerDisplay();

            yield return new WaitForSeconds(levelIntroDuration);

            isDisplayLevelIntro = false;

            levelIntroMgr.gameObject.SetActive(false);
            GameManager.instance.ActivateAnimator();
        }

        public void DisplayLevelOutro()
        {
            Debug.Log("OUTRO");

            //levelOutroMgr.Display();
            // TODO display step by step stats of the level and buttons to continue.
            // TODO automatic Save

            // Temporary.
            GameManager.instance.SwitchToWorldMap();
        }

        /// <summary>
        /// Trigger an internal transition (Switch to an another area in the same scene).
        /// </summary>
        /// <param name="newArea">New Area to reach.</param>
        /// <param name="newPosition">New position of Boing in this area.</param>
        public void PrepareInternalTransition(GameObject newArea, Vector3 newPosition)
        {
            isInternalTransition = true;

            nextArea = newArea;
            nextPosition = newPosition;
            nextPosition.z = boing.transform.position.z;

            GameManager.instance.TriggerFadeIn();
        }

        #endregion

        /// <summary>
        /// Move boing and camera to the prepared area and position.
        /// </summary>
        public void MoveBoingToNewArea()
        {
            isInternalTransition = false;

            boing.transform.position = nextPosition;
            cameraFollow.CurrentArea = nextArea.GetComponent<AreaEditor>();
            
            nextArea = null;
        }
        
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

        #endregion

        #region HUD management

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
    }
}