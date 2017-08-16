using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KekeDreamLand
{

    public class Mob : MonoBehaviour {

        public int damage = 1;

        // TODO life system or invincibility.

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.tag == "Player")
            {
                DealDamageToPlayer(collision.gameObject);
            }
        }

        private void DealDamageToPlayer(GameObject player)
        {
            player.GetComponent<BoingManager>().LifePoints--;
        }
    }
}