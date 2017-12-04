using UnityEngine;

namespace KekeDreamLand
{
    /// <summary>
    /// Monobehaviour of the bread. When the bread is take. Trigger end of the level.
    /// </summary>
    public class Bread : Item
    {
        public int exitIndex = 0;

        protected override void DoActionWhenPick()
        {
            // FEEDBACK -> sound, particles...

            // Trigger the end of the level for the specified exit.
            GameManager.instance.FinishCurrentLevel(exitIndex);

            Destroy(gameObject);
        }
    }
}