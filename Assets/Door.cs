using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KekeDreamLand
{

    public class Door : MonoBehaviour
    {

        public Door otherDoor;

        public GameObject currentArea;

        private bool canUseDoor;

        private void Awake()
        {
            currentArea = transform.parent.parent.parent.parent.gameObject;
            canUseDoor = true;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (otherDoor)

                if (other.tag == "Player" && canUseDoor)
                {
                    PreventUseDoor();
                    otherDoor.PreventUseDoor();

                    GameManager.instance.InternalTransition(otherDoor.currentArea, otherDoor.transform.position);
                }

                else
                    Debug.Log("No door associated on the door " + gameObject.name);
        }

        public void PreventUseDoor()
        {
            canUseDoor = false;

            StartCoroutine(CoolDownDoor());
        }

        // Run cooldown to permit to the player to reuse the door after time.
        public IEnumerator CoolDownDoor()
        {
            yield return new WaitForSeconds(2.5f);

            canUseDoor = true;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawCube(transform.position, Vector3.one * 0.2f);

            if (otherDoor)
                Gizmos.DrawLine(transform.position, otherDoor.transform.position);
        }
    }
}