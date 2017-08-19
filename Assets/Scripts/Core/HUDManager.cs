using UnityEngine;
using UnityEngine.UI;

namespace KekeDreamLand
{
    /// <summary>
    /// Manager of the HUD. Display Life points and other value. 
    /// </summary>
    public class HUDManager : MonoBehaviour
    {
        #region Inspector attributes
        
        public Text featherText;
        public Text lifePointsText;

        #endregion

        #region Private attributes

        private Animator hudAnimator;
        private bool displayed;

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
        /// Update the amount of lifepoints in the HUD.
        /// </summary>
        /// <param name="lifePoints">New amount</param>
        public void UpdateLifePoints(int lifePoints)
        {
            lifePointsText.text = lifePoints.ToString();

            // TODO Add animation or effect.
        }

        /// <summary>
        /// Update the amount of feather picked up.
        /// </summary>
        /// <param name="newAmount"></param>
        /// <param name="count"></param>
        public void UpdateFeatherPickedUp(int newAmount, int count)
        {
            featherText.text = newAmount + " / " + count;
        }

        #endregion
    }

}