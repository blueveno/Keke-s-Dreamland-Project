using System.Collections;
using UnityEngine;

namespace KekeDreamLand
{
    /// <summary>
    /// Path that linked two nodes of a world map.
    /// </summary>
    public class Path : MonoBehaviour
    {
        public bool unlocked = false;
        public bool secretLevel = false;

        /// <summary>
        /// Unlock the path and display it immediatly.
        /// </summary>
        public void DisplayPath()
        {
            foreach (Transform t in transform)
            {
                t.gameObject.SetActive(true);
            }

            unlocked = true;
        }

        /// <summary>
        /// Unlock the path and display it step by step.
        /// </summary>
        public IEnumerator UnlockPath()
        {
            foreach(Transform t in transform)
            {
                t.gameObject.SetActive(true);
                yield return new WaitForSeconds(0.1f);
            }

            unlocked = true;
        }
    }
}