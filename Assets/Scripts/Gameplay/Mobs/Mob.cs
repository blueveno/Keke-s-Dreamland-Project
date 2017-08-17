using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KekeDreamLand
{
    // TODO life system or invincibility.

    /// <summary>
    /// Attach this to any enemy in the game. Inherit if the mob need a specialized behaviour.
    /// </summary>
    public class Mob : MonoBehaviour {

        #region Inspector Attributes

        [Header("Mob configuration :")]
        public int damage = 1;
        #endregion

        #region Unity methods

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "Player")
            {
                DealDamageToPlayer(collision.gameObject);
            }
        }

        #endregion

        private void DealDamageToPlayer(GameObject player)
        {
            player.GetComponent<BoingManager>().LifePoints -= damage;
        }
    }
}