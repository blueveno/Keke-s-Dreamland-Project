using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractableGameobject : MonoBehaviour {

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            // TODO display fedback "Press button".

            // TODO notify that an interactable gameobject is in range.
            other.GetComponent<BoingManager>().InteractableGoInRange = this;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            // TODO notify that an interactable gameobject is no longer in range.
            other.GetComponent<BoingManager>().InteractableGoInRange = null;
        }
    }

    public abstract void DoActionWhenUse();
}
