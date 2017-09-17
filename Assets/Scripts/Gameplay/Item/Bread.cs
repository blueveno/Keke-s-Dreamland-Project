using UnityEngine;

namespace KekeDreamLand
{
    /// <summary>
    /// Monobehaviour of the bread. When the bread is take. Trigger end of the level.
    /// </summary>
    public class Bread : Item
    {
        protected override void DoActionWhenPick()
        {
            // FEEDBACK -> sound, particles...

            // Trigger the end of the level.
            GameManager.instance.FinishCurrentLevel();
        }
    }
}