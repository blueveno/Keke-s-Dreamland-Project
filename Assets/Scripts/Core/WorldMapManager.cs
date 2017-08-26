using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KekeDreamLand
{
    public class WorldMapManager : MonoBehaviour
    {
        // World map = Tree.
        
        // TODO load only world where the player is.

        // TODO change animator controller for another effect ?

        private void Update()
        {
            if (Input.GetButtonDown("Jump"))
            {
                GameManager.instance.TriggerFadeIn();
            }
        }

        public void SwitchToNewLevel()
        {
            GameManager.instance.SwitchToNewLevel(1, 1);
        }
    }

}
