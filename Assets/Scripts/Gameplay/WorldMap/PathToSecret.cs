using System.Collections;
using UnityEngine;
using TMPro;

namespace KekeDreamLand
{
    /// <summary>
    /// Path that linked two nodes of a world map.
    /// </summary>
    public class PathToSecret : Path
    {
        [Header("path to secret level")]
        public GameObject ui;
        public TextMeshProUGUI uiText;

        public void SetupUI(int sunflowerSeedCollected, int sunflowerSeedNeeded)
        {
            uiText.text = sunflowerSeedCollected + "/" + sunflowerSeedNeeded;
            ui.SetActive(true);
        }

        /// <summary>
        /// Unlock path directly.
        /// Color it in white or keep it transparent depending is the secret level has been unlocked.
        /// </summary>
        /// <param name="secretLevelUnlocked"></param>
        public void DisplaySecretPath(bool secretLevelUnlocked)
        {
            foreach (Transform t in transform)
            {
                t.gameObject.SetActive(true);

                // No transparency if the path to the secret level is unlocked.
                if (secretLevelUnlocked)
                    t.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
            }

            unlocked = secretLevelUnlocked;
        }

        /// <summary>
        /// Unlock path progressively.
        /// Color it in white or keep it transparent depending is the secret level has been unlocked.
        /// </summary>
        /// <param name="secretLevelUnlocked"></param>
        /// <returns></returns>
        public IEnumerator UnlockSecretPath(bool secretLevelUnlocked)
        {
            foreach (Transform t in transform)
            {
                t.gameObject.SetActive(true);

                // No transparency if the path to the secret level is unlocked.
                if (secretLevelUnlocked)
                    t.gameObject.GetComponent<SpriteRenderer>().color = Color.white;

                yield return new WaitForSeconds(0.1f);
            }

            unlocked = secretLevelUnlocked;
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