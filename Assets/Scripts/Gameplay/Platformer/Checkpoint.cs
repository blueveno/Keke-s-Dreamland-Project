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

                    validated = true;
                }
        }

        private void OnDrawGizmos()
        {
            if (validated) {
                Color green = Color.green;
                green.a = 0.2f;
                Gizmos.color = green;

                BoxCollider2D b = GetComponent<BoxCollider2D>();
                Gizmos.DrawCube(b.transform.position, b.size);
            }
        }
    }
}
