using UnityEngine;

namespace KekeDreamLand
{
    /// <summary>
    /// Kill the player when he enter in the hitbox of the attached gameobject.
    /// </summary>
    public class KillZone : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == "Player")
            {
                // Dont show the destruction to the player.
                Destroy(other.gameObject, 0.2f);

                GameManager.instance.FadeInAndReload();
            }
        }
    }
}
