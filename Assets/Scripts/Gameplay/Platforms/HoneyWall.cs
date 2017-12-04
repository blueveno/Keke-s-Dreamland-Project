using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KekeDreamLand
{
    public class HoneyWall : MonoBehaviour, IAction
    {
        #region inspector attributes

        [Tooltip("Number of hits to receive before being destroyed")]
        public int durability = 3;

        #endregion

        #region Private attributes

        private int currentDurability;

        #endregion

        #region Unity methods

        // Use this for initialization
        void Awake()
        {
            ResetWall();
        }

        #endregion

        /// <summary>
        /// Decrease one durability when boing peck this honey wall. "Destroy" it if durability reachs 0.
        /// </summary>
        public void OnPeck()
        {
            currentDurability--;

            if (currentDurability > 0)
            {
                StartCoroutine(ShakeSprite());
            }

            else
            {
                gameObject.SetActive(false);

                GameManager.instance.levelMgr.RegisterAction(this);
            }
        }
        
        private void ResetWall()
        {
            currentDurability = durability;
        }

        // Shake the sprite.
        private IEnumerator ShakeSprite()
        {
            GameObject sprite = transform.Find("Sprite").gameObject;

            Vector3 shakingPos = Vector3.zero;

            int shakingCount = 10;
            float shakeDuration = 0.1f / shakingCount;

            int i = 0;
            while (i < shakingCount)
            {
                shakingPos = Random.insideUnitCircle * 0.05f;

                // Change local position to simulate shake.
                sprite.transform.localPosition = shakingPos;

                yield return new WaitForSeconds(shakeDuration);
                i++;
            }

            // Reset position of the sprite.
            sprite.transform.localPosition = Vector3.zero;
        }

        public void DeleteAction()
        {
            Destroy(gameObject);
        }

        public void CancelAction()
        {
            ResetWall();
            gameObject.SetActive(true);
        }
    }
}