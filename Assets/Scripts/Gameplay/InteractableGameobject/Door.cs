using System;
using System.Collections;
using UnityEngine;

namespace KekeDreamLand
{
    public class Door : InteractableGameobject
    {
        [Tooltip("Door in an another area linked to this door.")]
        public Door otherDoor;
        public bool displayTransition = true;

        private GameObject currentArea;
        public GameObject CurrentArea
        {
            get { return currentArea; }
        }

        private bool canUseDoor;
        

        private void Awake()
        {
            currentArea = transform.parent.parent.parent.parent.gameObject;
            canUseDoor = true;
        }

        public override void DoActionWhenUse()
        {
            UseDoor();
        }

        private void UseDoor()
        {
            if (!otherDoor)
            {
                Debug.Log("No door associated to the door " + gameObject.name);
                return;
            }
            
            // TODO remove these preventions ?
            PreventUseDoor();
            otherDoor.PreventUseDoor();

            GameManager.instance.InternalTransition(otherDoor.currentArea, otherDoor.transform.position);
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

        // Level design helper.
        private void OnDrawGizmos()
        {
            if(displayTransition)
            {
                DrawTransition();
            }
        }

        private void DrawTransition()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawCube(transform.position, Vector3.one * 0.2f);

            if (otherDoor)
                Gizmos.DrawLine(transform.position, otherDoor.transform.position);

            Gizmos.color = Color.red;
            Gizmos.DrawCube(otherDoor.transform.position, Vector3.one * 0.2f);
        }
    }
}