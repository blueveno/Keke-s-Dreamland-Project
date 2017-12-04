using System.Collections;
using UnityEngine;
using System.Collections.Generic;

namespace KekeDreamLand
{
    /// <summary>
    /// Path that linked two nodes of a world map.
    /// </summary>
    public class Path : MonoBehaviour
    {
        public bool unlocked = false;

        public List<Vector2> waypoints;

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

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.cyan;
            foreach(Vector3 point in waypoints)
            {
                Gizmos.DrawSphere(point, 0.05f);
            }
        }
    }
}