using System.Collections;
using UnityEngine;

namespace KekeDreamLand
{

    public abstract class InteractableGameobject : MonoBehaviour
    {
        public float useCooldown = 0.5f;

        private bool canInteract = true;

        // Indicates to Boing that he enters in range of this interactable gameobject.
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == "Player")
            {
                EnterInRange(other.gameObject);
            }
        }

        // Indicates to Boing that he stays in range of this interactable gameobject.
        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.tag == "Player")
            {
                EnterInRange(other.gameObject);
            }
        }

        private void EnterInRange(GameObject player)
        {
            if (player.GetComponent<BoingManager>().InteractableGoInRange == this)
                return;

            else
            {
                player.GetComponent<BoingManager>().InteractableGoInRange = this;

                // TODO display feedback "Press button" ?

                // Get center.x and max.y of boxcollider2D to always display 1 unit at top of the interactable gameobject.
            }

        }

        // Indicates to Boing that he is no longer in range of this interactable gameobject.
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.tag == "Player")
            {
                other.GetComponent<BoingManager>().InteractableGoInRange = null;

                // TODO undisplay feedback "Press button" ?
            }
        }

        public void Interact()
        {
            if (!canInteract)
                return;

            canInteract = false;
            StartCoroutine(InteractDelay());
            DoActionWhenUse();
        }

        // Action opered when the interactable gameobject is used.
        protected abstract void DoActionWhenUse();

        private IEnumerator InteractDelay()
        {
            yield return new WaitForSeconds(useCooldown);
            canInteract = true;
        }
    }

}