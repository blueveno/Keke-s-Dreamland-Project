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
            TriggerEnd();

            // FEEDBACK -> sound, particles...
        }

        // Trigger the end of the level.
        private void TriggerEnd()
        {
            GameManager.instance.FinishCurrentLevel();
        }
    }
}