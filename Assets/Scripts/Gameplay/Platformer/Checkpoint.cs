using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KekeDreamLand
{
    public class Checkpoint : MonoBehaviour
    {
        [Tooltip("Put here the area concerned by this checkpoint")]
        public GameObject areaGameObject;
        public Animator anim;

        private bool validated;

        // TODO sprite, animation when checkpoint reached.

        private void Awake()
        {
            if(areaGameObject == null)
            {
                areaGameObject = transform.parent.parent.parent.parent.gameObject;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!validated)
            {
                if (collision.tag == "Player")
                {
                    validated = true;

                    if (anim)
                    {
                        anim.SetTrigger("Bounce");
                    }

                    GameManager.instance.levelMgr.LastCheckPoint = this;
                }
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
