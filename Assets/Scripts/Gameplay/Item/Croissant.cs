using UnityEngine;

namespace KekeDreamLand
{
    /// <summary>
    /// Item which heal one life point to Boing when it is picked.
    /// </summary>
    public class Croissant : Item
    {
        protected override void DoActionWhenPick()
        {
            // FEEDBACK -> sound, particles...

            // Notify that a "croissant" has been picked up.
            GameManager.instance.CurrentLevel.PickCroissant();
        }
    }
}