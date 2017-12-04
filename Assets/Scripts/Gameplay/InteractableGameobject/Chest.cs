using System.Collections;
using UnityEngine;

namespace KekeDreamLand
{
    /// <summary>
    /// A chest is an interactable object which can be open only if the player has got the key.
    /// </summary>
    public class Chest : InteractableGameobject, IAction
    {
        #region Inspector attributes

        [Tooltip("Treasure contained in this chest.")]
        public Treasure treasure;

        #endregion

        #region Private attributes

        private Animator anim;
        private SpriteRenderer treasureRenderer;

        private bool isUsed;
        
        /// <summary>
        /// Return true if chest has been opened.
        /// </summary>
        public bool Opened
        {
            get { return opened; }
            set
            {
                opened = value;
                if (opened)
                {
                    anim.SetTrigger("Open");
                } else
                {
                    anim.SetTrigger("Lost");
                }
            }
        }
        private bool opened = false;

        #endregion

        #region Unity methods

        private void Awake()
        {
            anim = GetComponent<Animator>();
            treasureRenderer = transform.Find("Treasure").gameObject.GetComponent<SpriteRenderer>();

            // Change treasure sprite depending data.
            treasureRenderer.sprite = treasure.sprite;
        }

        private void Start()
        {
            // Define chest of the level.
            GameManager.instance.levelMgr.Chest = this;
        }

        #endregion

        protected override void DoActionWhenUse()
        {
            // Do nothing if chest already opened.
            if (opened || isUsed)
                return;

            // Open chest if the player has the key.
            if (GameManager.instance.levelMgr.TryToOpenChest())
            {
                Opened = true;
            }

            // Shake the chest when player hasn't the key.
            else
            {
                StartCoroutine(ShakeSprite());
            }
        }

        // Shake the sprite.
        private IEnumerator ShakeSprite()
        {
            isUsed = true;

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
            isUsed = false;
        }

        // Chest is validated.
        public void DeleteAction()
        {
            
        }

        // Chest is reset
        public void CancelAction()
        {
            Opened = false;
            GameManager.instance.levelMgr.UpdateTreasureInHUD(false);
        }
    }
}
