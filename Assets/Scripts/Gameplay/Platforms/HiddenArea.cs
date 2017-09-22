using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KekeDreamLand
{
    /// <summary>
    /// An hidden area is a set of midground tiles which is hidden by a set of foreground tiles.
    /// Boing reveal the area when he pass behind one of the tiles.
    /// </summary>
    public class HiddenArea : MonoBehaviour
    {
        #region Inspector attributes

        [Tooltip("Put here the set of foreground tiles of the hidden area.")]
        public GameObject hiddenAreaTiles;

        #endregion

        #region Private attributes

        private float currentOpacity = 1.0f;

        #endregion

        #region Unity methods

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag == "Player")
            {
                StopAllCoroutines();

                StartCoroutine(RevealHiddenArea(false));
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.tag == "Player")
            {
                StopAllCoroutines();

                StartCoroutine(RevealHiddenArea(true));
            }
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Reveal or hide (if reverse) the set of tiles.
        /// </summary>
        /// <param name="reverse">Reverse effect</param>
        /// <returns></returns>
        private IEnumerator RevealHiddenArea(bool reverse)
        {
            Color c = Color.white;
            c.a = currentOpacity;

            while ((c.a > 0.1f && !reverse) || (c.a < 1.0f && reverse))
            {

                if (reverse)
                    c.a += 0.1f;
                else
                    c.a -= 0.1f;

                currentOpacity = c.a;

                foreach (Transform t in hiddenAreaTiles.transform)
                {
                    t.gameObject.GetComponent<SpriteRenderer>().color = c;
                }

                yield return new WaitForSeconds(0.05f);
            }
        }

    }

    #endregion
}