using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace KekeDreamLand {

    public class WorldMapHUDManager : MonoBehaviour
    {
        #region Inspector attributes

        [Header("Node preview")]
        public Text nodeName;
        public TextMeshProUGUI nodeName2;

        [Header("Level preview")]
        public GameObject levelPreview;
        [Space]
        public TextMeshProUGUI feathers;
        [Space]
        public GameObject specialItemParent;
        public Color notFoundColor;

        #endregion

        #region Private attributes

        #endregion

        #region Public methods

        /// <summary>
        /// Update the level preview and informations about the level.
        /// </summary>
        /// <param name="nodeName">Node information.</param>
        /// <param name="levelData">Data of this level. null if the node is not a level.</param>
        /// <param name="levelProgress">Current progress of the player in this level. null if no progress.</param>
        public void UpdateLevelPreview(string nodeName, LevelData levelData, LevelProgress levelProgress)
        {
            // Update what is this node.
            this.nodeName.text = nodeName;

            bool levelInfoDisplayed = false;
            // Display level infos.
            if (levelData != null)
            {
                // Update level infos.
                nodeName2.text = levelData.levelName;
                nodeName2.fontSize = (levelData.levelName.Length > 18) ? 45 : 55;

                int featherCollected = 0;
                // Try to get progress.
                if (levelProgress != null)
                    featherCollected = levelProgress.feathersCollected;

                // Update feathers infos.
                feathers.text = featherCollected + " / " + levelData.totalFeathers;

                // Special item info.
                int i = 0;
                foreach(Transform child in specialItemParent.transform)
                {
                    // Display or hide depending if the item is present in the level.
                    child.gameObject.SetActive(levelData.itemsPresent[i]);

                    // Check if the special item is present on the level.
                    if (levelData.itemsPresent[i])
                    {
                        Image img = child.gameObject.GetComponent<Image>();

                        // Save exist 
                        if (levelProgress != null)
                        {
                            // Item found, highlight it.
                            if (levelProgress.specialItemsFound[i])
                                img.color = Color.white;

                            // Item not found, remove highlight.
                            else
                                img.color = notFoundColor;
                        }

                        // No save. No highlight.
                        else
                            img.color = notFoundColor;
                    }

                    i++;
                } 

                levelInfoDisplayed = true;
            }

            DisplayLevelInfos(levelInfoDisplayed);
        }

        #endregion

        #region Private methods
        
        // Remove useless informations when display an other type of node.
        private void DisplayLevelInfos(bool displayed)
        {
            levelPreview.SetActive(displayed);
            nodeName2.gameObject.SetActive(displayed);
        }

        #endregion
    }
}