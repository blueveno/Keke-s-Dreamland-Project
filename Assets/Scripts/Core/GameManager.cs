// Dont delete for list or coroutine.
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace KekeDreamLand
{
    public class GameManager : MonoBehaviour
    {
        public int worldmapIndex;

        #region GameManager Attributes

        // Singleton.
        public static GameManager instance = null;

        // Boing gameobject and scripts...
        private GameObject boing;

        private GameObject ui;

        private CustomCamera2DFollow cameraFollow;

        private TransitionManager transitionManager;

        private GameObject nextArea;

        private bool isEndOfLevel;
        private bool isInternalTransition;

        #endregion

        #region Unity methods

        private void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(gameObject);

            DontDestroyOnLoad(gameObject);

            SceneManager.sceneLoaded += NewSceneLoaded;

            SetupLevel();
        }

        private void NewSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            SetupLevel();
        }

        private void SetupLevel()
        {
            // Gameobject or script.
            boing = GameObject.FindGameObjectWithTag("Player");

            cameraFollow = Camera.main.GetComponent<CustomCamera2DFollow>();

            ui = GameObject.FindGameObjectWithTag("UI");
            if(ui)
                transitionManager = ui.transform.Find("TransitionPanel").GetComponent<TransitionManager>();

            // Other
            isEndOfLevel = false;
            isInternalTransition = false;
        }

        #endregion

        #region Public methods

        public void FinishLevel()
        {
            isEndOfLevel = true;
            transitionManager.FadeIn();
        }

        // Use this when player die or fall.
        public void RestartScene()
        {
            if(transitionManager)
                transitionManager.FadeIn();
        }

        // Event triggered when fadeInTransition finished.
        public void FadeInFinished()
        {
            // Case of an end of level.
            if (isEndOfLevel)
                NextLevel();

            // Case of an internal transition.
            else if (isInternalTransition)
            {
                isInternalTransition = false;
                transitionManager.FadeOut();

                cameraFollow.CurrentArea = nextArea.GetComponent<LevelEditor>();

                nextArea = null;
            }

            // Case of a restart of the level.
            else
                ResetCurrentScene();
        }

        public void InternalTransition(GameObject newArea, Vector3 newPosition)
        {
            isInternalTransition = true;

            transitionManager.FadeIn();

            nextArea = newArea;

            boing.transform.position = newPosition;
        }

        #endregion

        #region Scene management

        private void NextLevel()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
        }

        private void ResetCurrentScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        private void SwitchToWorldMap()
        {
            SceneManager.LoadScene(worldmapIndex);
        }

        #endregion
    }
}