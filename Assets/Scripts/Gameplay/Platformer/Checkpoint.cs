using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KekeDreamLand
{
    public class Checkpoint : MonoBehaviour
    {
        private bool validated;

        // TODO sprite, animation when checkpoint reached.

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!validated)
                if (collision.tag == "Player")
                {
                    GameManager.instance.CurrentLevel.LastCheckPoint = this;

                    Debug.Log("Checkpoint reached.");

                    validated = true;
                }
        }
    }
}
