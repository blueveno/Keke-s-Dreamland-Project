using System;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        private void TriggerEnd()
        {
            Debug.LogWarning("END OF THE LEVEL");

            GameManager.instance.RestartScene();
        }
    }
}