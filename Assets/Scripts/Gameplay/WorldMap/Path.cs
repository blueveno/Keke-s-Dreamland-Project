using UnityEngine;

namespace KekeDreamLand
{
    /// <summary>
    /// Path that linked two nodes of a world map.
    /// </summary>
    public class Path : MonoBehaviour
    {
        public bool unlocked = false;

        /// <summary>
        /// Unlock the path and also display it.
        /// </summary>
        public void UnlockPath()
        {
            unlocked = true;

            foreach(Transform t in transform)
            {
                t.gameObject.SetActive(true);
            }
        }
    }
}