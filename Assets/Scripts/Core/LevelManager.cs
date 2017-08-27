using UnityEngine;

namespace KekeDreamLand
{
    /// <summary>
    /// Manage a level. Setup, internal transition, level HUD.
    /// </summary>
    public class LevelManager : MonoBehaviour
    {
        #region LevelManager Attributes

        // Boing
        private GameObject boing;
        private BoingManager boingScript;

        // HUD
        private HUDManager hudManager;

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
            get { return isLevelFinished || isInternalTransition; }
        }

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
                hudManager = ui.transform.Find("HUD").GetComponent<HUDManager>();
            
            CountFeathersInCurrentLevel();

            // Special items.
            for (int i = 0; i < specialItemPresent.Length; i++)
            {
                specialItemPresent[i] = false;
            }
            CheckSpecialItemsPresent();

            // Update level HUD.
            UpdateLifePoints(boingScript.maxLifePoints);
            hudManager.SetupFeatherIndicators(featherCount);

            hudManager.SetupSpecificItem(hudManager.KeySprite, specialItemPresent[0]);
            hudManager.SetupSpecificItem(hudManager.RaisinBreadSprite, specialItemPresent[1]);
            hudManager.SetupSpecificItem(hudManager.ChocolatineSprite, specialItemPresent[2]);
            hudManager.SetupSpecificItem(hudManager.SunflowerSeedSprite, specialItemPresent[3]);
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

                else if (item.name.Contains("Raisin Bread"))
                {
                    specialItemPresent[1] = true;
                }

                else if (item.name.Contains("Chocolatine"))
                {
                    specialItemPresent[2] = true;
                }

                else if (item.name.Contains("Sunflower seed"))
                {
                    specialItemPresent[3] = true;
                }
            }
        }

        #endregion

        #region Level management

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

        #endregion

        // TODO move also HUD ?

        #region HUD management

        /// <summary>
        /// Request HUD to be toggled.
        /// </summary>
        public void ToggleHUD()
        {
            hudManager.ToggleHUD();
        }

        /// <summary>
        /// Request lifepoints to be updated in hud.
        /// </summary>
        public void UpdateLifePoints(int lifePoints)
        {
            hudManager.UpdateLifePoints(lifePoints);
        }

        private void RefreshFeatherCount()
        {
            hudManager.UpdateFeatherPickedUp(featherPickedUp, featherCount);
        }

        #endregion
    }
}