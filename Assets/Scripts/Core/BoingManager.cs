using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoingManager : MonoBehaviour {

    private InteractableGameobject interactableGoInRange;
    public InteractableGameobject InteractableGoInRange
    {
        get { return interactableGoInRange; }
        set { interactableGoInRange = value; }
    }

    private void Awake()
    {
        interactableGoInRange = null;
    }

}
