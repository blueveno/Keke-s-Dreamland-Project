using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KekeDreamLand
{

    public class Chest : InteractableGameobject
    {
        public Treasure treasure;
        
        private Animator anim;
        private SpriteRenderer treasureRenderer;

        private bool isUsed;
        private bool opened = false;

        private void Awake()
        {
            anim = GetComponent<Animator>();
            treasureRenderer = transform.Find("Treasure").gameObject.GetComponent<SpriteRenderer>();

            // Change treasure sprite depending data.
            treasureRenderer.sprite = treasure.sprite;
        }

        protected override void DoActionWhenUse()
        {
            // Do nothing if chest already opened.
            if (opened || isUsed)
                return;

            isUsed = true;

            // Open chest.
            if(GameManager.instance.CurrentLevel.HasTheKey())
            {
                opened = true;
                anim.SetTrigger("Open");
                
                // Get treasure.
                Debug.Log("Got " + treasure.treasureName);
                // TODO notify level update HUD.
            }

            // Feedback that chest is locked.
            else
            {
                // Shake chest when Boing hasn't the key.
                StartCoroutine(ShakeSprite());

                Debug.Log("You don't have the key !!!");
            }
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
            isUsed = false;
        }
    }
}
