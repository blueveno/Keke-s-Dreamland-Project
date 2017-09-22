using UnityEngine;

namespace KekeDreamLand
{
    /// <summary>
    /// Kill the player when he enter in the hitbox of the attached gameobject.
    /// </summary>
    public class KillZone : MonoBehaviour
    {
        public bool isMovingKillzone = false;
        public float deathDelay = 0.2f;

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == "Player")
            {
                BoingManager boing = other.gameObject.GetComponent<BoingManager>();

                // Kill boing after a delay.
                boing.Die(deathDelay);
            }

            // Destroy anything else which enter in this killzone. Except if it's the killzone of a forced scrolling area.
            else
            {
                if(!isMovingKillzone)
                    Destroy(other.gameObject, deathDelay);
            }
        }
    }
}
