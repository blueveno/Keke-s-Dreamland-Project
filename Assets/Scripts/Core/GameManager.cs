// Dont delete for list or coroutine.
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace KekeDreamLand
{
    /// <summary>
    /// Manage game. Don't destroy on load.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        [Tooltip("build index of the world map scene.")]
        public int worldMapIndex;

        #region GameManager Attributes

        // Singleton.
        public static GameManager instance = null;

        // Boing gameobject and scripts...
        private GameObject boing;

        // Ui
        private GameObject ui;
        private HUDManager hudManager;

        // Camera follow script.
        private CustomCamera2DFollow cameraFollow;

        // Transition script.
        private TransitionManager transitionManager;

        // internal transition attributes.
        private GameObject nextArea;
        private Vector3 nextPosition;

        // Game manager attributes.
        private bool isEndOfLevel;
        private bool isInternalTransition;

        #endregion

        #region Unity methods

        private void Awake()
        {
            SingletonThis();

            SceneManager.sceneLoaded += NewSceneLoaded;

            SetupLevel();
        }

        #endregion

        #region Private methods

        // Singleton this class.
        private void SingletonThis()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(gameObject);

            DontDestroyOnLoad(gameObject);
        }

        // Delegate method triggered when a new scene is loaded.
        private void NewSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            SetupLevel();
        }

        // Setup all components and variables of the gamemanager.
        private void SetupLevel()
        {
            // Gameobject or script.
            boing = GameObject.FindGameObjectWithTag("Player");

            cameraFollow = Camera.main.GetComponent<CustomCamera2DFollow>();

            ui = GameObject.FindGameObjectWithTag("UI");
            if (ui)
            {
                transitionManager = ui.transform.Find("TransitionPanel").GetComponent<TransitionManager>();
                hudManager = ui.transform.Find("HUD").GetComponent<HUDManager>();
            }

            // Other
            isEndOfLevel = false;
            isInternalTransition = false;
        }

        #endregion

        #region Game management

        /// <summary>
        /// Use this when you want to finish the current level.
        /// </summary>
        public void FinishLevel()
        {
            isEndOfLevel = true;

            transitionManager.FadeIn();
        }

        /// <summary>
        /// Use this when you want to fade in and reload the current scene.
        /// </summary>
        public void FadeInAndReload()
        {
            transitionManager.FadeIn();
        }

        /// <summary>
        /// Trigger an internal transition (Switch to an another area in the same scene).
        /// </summary>
        /// <param name="newArea">New Area to reach.</param>
        /// <param name="newPosition">New position of Boing in this area.</param>
        public void TriggerInternalTransition(GameObject newArea, Vector3 newPosition)
        {
            isInternalTransition = true;

            transitionManager.FadeIn();

            nextArea = newArea;
            nextPosition = newPosition;
        }

        /// <summary>
        /// Event triggered when a fadeIn transition animation finished.
        /// </summary>
        public void FadeInFinished()
        {
            // Case of an end of level.
            if (isEndOfLevel)
                NextLevel();

            // Case of an internal transition.
            else if (isInternalTransition)
            {
                isInternalTransition = false;

                // Move Boing and Camera view when user don't see.
                boing.transform.position = nextPosition;
                cameraFollow.CurrentArea = nextArea.GetComponent<AreaEditor>();

                transitionManager.FadeOut();

                nextArea = null;
            }

            // Case of a restart of the level.
            else
                ResetCurrentScene();
        }

        #endregion

        #region Scene management

        // Switch to the next scene.
        private void NextLevel()
        {
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);

            // TEMPORARY loop with the current loaded scene.
            ResetCurrentScene();
        }

        // Switch to the world map scene.
        private void SwitchToWorldMap()
        {
            SceneManager.LoadScene(worldMapIndex);
        }

        // Reset current loaded scene.
        private void ResetCurrentScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        #endregion

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

        #endregion
    }
}