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
                // TODO display fedback "Press button".

                other.GetComponent<BoingManager>().InteractableGoInRange = this;
            }
        }

        // Indicates to Boing that he stays in range of this interactable gameobject.
        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.tag == "Player")
            {
                // TODO display fedback "Press button".

                if (other.GetComponent<BoingManager>().InteractableGoInRange != this)
                    other.GetComponent<BoingManager>().InteractableGoInRange = this;
            }
        }

        // Indicates to Boing that he is no longer in range of this interactable gameobject.
        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.tag == "Player")
            {
                other.GetComponent<BoingManager>().InteractableGoInRange = null;
            }
        }

        // Action opered when the interactable gameobject is used.
        public abstract void DoActionWhenUse();
    }

}