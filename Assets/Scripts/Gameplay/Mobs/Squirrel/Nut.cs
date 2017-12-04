using UnityEngine;

namespace KekeDreamLand
{
    /// <summary>
    /// Nut is a projectile throw by the squirrel.
    /// </summary>
    public class Nut : Mob
    {
        protected new void OnTriggerEnter2D(Collider2D collision)
        {
            bool destroyed = false;

            // Damage player if enter in collision with. Only if the mob is not bouncing.
            if (collision.gameObject.tag == "Player")
            {
                DealDamageToPlayer(collision.gameObject);
                destroyed = true;
            }
            
            destroyed |= collision.gameObject.tag == "OutOfBound" || collision.gameObject.tag == "Untagged";

            // The nut is destroyed when it hits the player, ground or reaches a bound of bound wall.
            if (destroyed)
                Destroy(gameObject, 0.02f);
        }
    }

}