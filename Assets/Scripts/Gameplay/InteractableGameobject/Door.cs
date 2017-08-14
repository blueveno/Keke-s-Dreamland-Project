using System.Collections;
using UnityEngine;

namespace KekeDreamLand
{
    // TODO : feedback "Press button" - Sound - animation of Boing when he pass a door...

    /// <summary>
    /// Door behaviour. Player can use it when he is in front of it to reach an another area.
    /// </summary>
    public class Door : InteractableGameobject
    {
        #region Public Door attributes

        [Tooltip("Door in an another area linked to this door.")]
        public Door doorLinked;
        public bool displayTransition = true;
        
        #endregion
        
        #region Private Door attributes

        // Area which contains this door.
        public GameObject CurrentArea
        {
            get { return currentArea; }
        }
        private GameObject currentArea;

        private bool canUseDoor;

        #endregion

        // Setup the door.
        private void Awake()
        {
            currentArea = transform.parent.parent.parent.parent.gameObject;
            canUseDoor = true;
        }

        /// <summary>
        /// Use door if possible.
        /// </summary>
        public override void DoActionWhenUse()
        {
            if (!canUseDoor)
            {
                return;
            }

            UseDoor();
        }

        // Switch to an another area when using the door. Error if destination is not associated.
        private void UseDoor()
        {
            if (!doorLinked)
            {
                Debug.Log("Destination door is not associated to this door (" + gameObject.name +")");
                return;
            }
            
            // TODO remove these preventions ?
            PreventUseDoor();
            doorLinked.PreventUseDoor();

            GameManager.instance.TriggerInternalTransition(doorLinked.currentArea, doorLinked.transform.position);
        }

        // Anti spam. TODO in another way ? Stop time and input interaction during animation ?
        public void PreventUseDoor()
        {
            canUseDoor = false;

            StartCoroutine(CoolDownDoor());
        }

        // Run cooldown to permit to the player to reuse the door after time.
        public IEnumerator CoolDownDoor()
        {
            yield return new WaitForSeconds(2f);

            canUseDoor = true;
        }
        
        // Display on editor only.
        private void OnDrawGizmos()
        {
            if(displayTransition && doorLinked)
            {
                DrawTransition();
            }
        }

        // Draw a transition between the two doors to help the level designer.
        private void DrawTransition()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawCube(transform.position, Vector3.one * 0.2f);

            if (doorLinked)
                Gizmos.DrawLine(transform.position, doorLinked.transform.position);

            Gizmos.color = Color.red;
            Gizmos.DrawCube(doorLinked.transform.position, Vector3.one * 0.2f);
        }
    }
}