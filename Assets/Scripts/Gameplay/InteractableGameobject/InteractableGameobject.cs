using UnityEngine;

namespace KekeDreamLand
{

    public abstract class InteractableGameobject : MonoBehaviour
    {

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

        // Action opered when the interactable gameobject is used.
        public abstract void DoActionWhenUse();
    }

}