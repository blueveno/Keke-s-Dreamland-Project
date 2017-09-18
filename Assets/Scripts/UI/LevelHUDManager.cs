using UnityEngine;
using UnityEngine.UI;
using TMPro;

using System.Collections.Generic;

namespace KekeDreamLand
{
    /// <summary>
    /// Manager of the HUD. Display Life points and other value. 
    /// </summary>
    public class LevelHUDManager : MonoBehaviour
    {
        #region Inspector attributes
        
        [Header("Lifepoints :")]
        
        public Transform lifePointsParent;
        public GameObject lifePointSprite;

        [Header("Collectables :")]
        public GameObject featherParent;
        public TextMeshProUGUI featherText;

        [Space]

        public Image[] specialItemSprites = new Image[4];

        #endregion

        #region Private attributes

        private Animator hudAnimator;
        private bool displayed;

        private List<GameObject> lifePointsSprites = new List<GameObject>();

        #endregion

        #region Unity methods

        void Awake()
        {
            hudAnimator = GetComponent<Animator>();

            displayed = true;
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Display or undisplay HUD.
        /// </summary>
        public void ToggleHUD()
        {
            // Prevent spam.

            if (displayed)
            {
                hudAnimator.SetTrigger("MoveUp");
                displayed = false;
            }

            else
            {
                hudAnimator.SetTrigger("MoveDown");
                displayed = true;
            }
        }

        /// <summary>
        /// Update the amount of lifepoints sprites in the HUD.
        /// </summary>
        /// <param name="lifePoints">New amount</param>
        public void UpdateLifePoints(int lifePoints)
        {
            int spriteDiff = Mathf.Abs(lifePointsSprites.Count - lifePoints);

            // Remove sprites from the lifepoints HUD.
            if (lifePoints < lifePointsSprites.Count)
            {
                for (int i = 0; i < spriteDiff; i++)
                {
                    //GameObject spriteRemoved = lifePointsSprites[0];
                    // TODO trigger animation ?

                    Destroy(lifePointsSprites[0]);
                    lifePointsSprites.RemoveAt(0);
                }
            }

            // Add sprites to the lifepoints HUD.
            else
            {
                for (int i = 0; i < spriteDiff; i++)
                {
                    GameObject sprite = Instantiate(lifePointSprite, lifePointsParent);
                    lifePointsSprites.Add(sprite);
                }
            }

            // TODO add animation or effect.
        }

        /// <summary>
        /// Setup the feather indicators on the level HUD.
        /// </summary>
        /// <param name="count">Total number of collectable feathers.</param>
        public void SetupFeatherIndicators(int count)
        {
            if (count == 0)
            {
                featherParent.SetActive(false);
                return;
            }

            featherText.text = 0 + " / " + count;
        }

        /// <summary>
        /// Update the amount of feather picked up.
        /// </summary>
        /// <param name="newAmount">new amount of collected feathers.</param>
        /// <param name="count">Total number of collectable feathers.</param>
        public void UpdateFeatherPickedUp(int newAmount, int count)
        {
            featherText.text = newAmount + " / " + count;
        }

        /// <summary>
        /// Display the specific item indicator or not.
        /// </summary>
        /// <param name="specialItem">Which special item indicator to display.</param>
        /// <param name="enabled">Displayed or not</param>
        public void SetupSpecificItem(int i, bool displayed)
        {
            specialItemSprites[i].gameObject.SetActive(displayed);
        }

        /// <summary>
        /// Unlock a specific item.
        /// </summary>
        /// <param name="specialItem"></param>
        public void UnlockSpecialItem(int i)
        {
            // TODO trigger animation of Unlock ?
            specialItemSprites[i].color = Color.white;
        }

        #endregion
    }

}