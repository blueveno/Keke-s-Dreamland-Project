using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KekeDreamLand {

    public class Feather : Item
    {
        protected override void DoActionWhenPick()
        {
            GameManager.instance.FeatherPickedUp++;
        }
    }
}
