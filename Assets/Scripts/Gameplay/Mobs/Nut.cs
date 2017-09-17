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
            bool destroy = false;

            // Damage player if enter in collision with. Only if the mob is not bouncing.
            if (collision.gameObject.tag == "Player")
            {
                DealDamageToPlayer(collision.gameObject);
                destroy = true;
            }

            destroy |= collision.gameObject.tag == "OutOfBound"
                || collision.gameObject.tag == "Untagged"; // tiles are untagged - special tiles not.

            // The nut is destroyed when it hits the player, ground or reaches a bound of bound wall.
            if (destroy)
                Destroy(gameObject);
        }
    }

}