using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KekeDreamLand
{
    // TODO life system or invincibility.

    /// <summary>
    /// Attach this to any enemy in the game. Inherit if the mob need a specialized behaviour.
    /// </summary>
    public class Mob : MonoBehaviour {

        #region Inspector Attributes

        [Header("Mob configuration :")]
        public int damage = 1;

        public bool canBounce = false;
        public float BounceEffectDuration = 2.0f;
        #endregion

        #region Private attributes

        protected bool isBouncing;

        private float timer = 0.0f;

        #endregion

        #region Unity methods

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Damage player if enter in collision with. Only if the mob is not bouncing.
            if (!isBouncing && collision.gameObject.tag == "Player")
            {
                DealDamageToPlayer(collision.gameObject);
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Trigger bounce effect if affected.
        /// </summary>
        public void TriggerBounce()
        {
            if (canBounce)
                Bounce();
            else
                Debug.Log(name + " can't bounce !");

            // TODO feedback when not affected.
        }

        #endregion

        #region Private methods

        private void DealDamageToPlayer(GameObject player)
        {
            player.GetComponent<BoingManager>().LifePoints -= damage;
        }
        
        private void Bounce()
        {
            if (!isBouncing)
                StartCoroutine(BounceWhileAffected());

            // Reset cooldown.
            else
                timer = 0.0f;
        }

        private IEnumerator BounceWhileAffected()
        {
            isBouncing = true;
            ToggleIABehaviours(false);

            // TODO trigger bounce animation.
            // temporary : color sprite.
            GetComponent<SpriteRenderer>().color = Color.magenta;

            BoingManager player = GameObject.FindGameObjectWithTag("Player").GetComponent<BoingManager>();
            
            // Don't do anything while Boing bounce.
            yield return new WaitWhile(() => (player != null && player.IsBouncing));

            // Cooldown. Reset if Boing bounce.
            timer = 0.0f;
            while(timer <= BounceEffectDuration)
            {
                yield return new WaitForSeconds(0.5f);
                timer += 0.5f;
            }

            isBouncing = false;
            ToggleIABehaviours(true);

            // TODO Stop bounce animation.
            // temporary : uncolor sprite.
            GetComponent<SpriteRenderer>().color = Color.white;
        }

        private void ToggleIABehaviours(bool enabled)
        {
            AIBehaviour[] ias = GetComponents<AIBehaviour>();
            foreach(AIBehaviour ia in ias)
            {
                ia.enabled = enabled;
            }
        }

        #endregion
    }
}