using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KekeDreamLand
{
    /// <summary>
    /// Manager of Boing (Except Locomotion).
    /// </summary>
    public class BoingManager : MonoBehaviour
    {
        #region Inspector attributes

        public int maxLifePoints = 3;

        public float invulnerabilityDuration = 2.0f;

        #endregion

        #region Private attributes

        /// <summary>
        /// Current interactable gameobject in range of Boing. null if nothing is in range.
        /// </summary>
        public InteractableGameobject InteractableGoInRange
        {
            get { return interactableGoInRange; }
            set { interactableGoInRange = value; }
        }
        private InteractableGameobject interactableGoInRange;

        /// <summary>
        /// LifePoints of Boing. Set value to damage it. Example LifePoints-- or -= 2.
        /// </summary>
        public int LifePoints
        {
            get { return lifePoints; }

            set {
                lifePoints = value;

                if(lifePoints <= 0)
                {
                    lifePoints = 0;
                    Die();
                }

                else if (lifePoints > maxLifePoints)
                    lifePoints = maxLifePoints;

                // Update HUD.
                GameManager.instance.UpdateLifePoints(lifePoints);

                // TODO temprorary invulnerability.
            }
        }
        private int lifePoints; // max 3

        #endregion

        #region Unity methods

        private void Awake()
        {
            interactableGoInRange = null;

            lifePoints = maxLifePoints;
        }

        #endregion

        #region Private methods

        private void Die()
        {
            // TODO animation, sound, ...

            GameManager.instance.FadeInAndReload();
        }

        #endregion
    }
}