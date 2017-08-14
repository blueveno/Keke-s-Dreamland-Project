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
        /// <summary>
        /// Current interactable gameobject in range of Boing. null if nothing is in range.
        /// </summary>
        public InteractableGameobject InteractableGoInRange
        {
            get { return interactableGoInRange; }
            set { interactableGoInRange = value; }
        }
        private InteractableGameobject interactableGoInRange;

        private void Awake()
        {
            interactableGoInRange = null;
        }
    }
}