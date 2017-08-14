using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace KekeDreamLand
{
    public class KillZone : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == "Player")
            {
                // Dont show the destruction to the player.
                Destroy(other.gameObject, 0.2f);

                GameManager.instance.RestartScene();
            }
        }
    }
}
