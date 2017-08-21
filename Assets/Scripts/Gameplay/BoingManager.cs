using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KekeDreamLand
{
    /// <summary>
    /// Manager of Boing (Except Locomotion).
    /// </summary>
    public class BoingManager : MonoBehaviour
    {
        #region Inspector attributes

        [Header("Life system")]
        public int maxLifePoints = 3;

        public float invulnerabilityDuration = 2.0f;

        [Header("Bouncing")]
        public float bouncingEffectRadius = 2.5f;
        [SerializeField]
        private LayerMask whatIsMobs;

        #endregion

        #region Private attributes

        /// <summary>
        /// Current interactable gameobject in range of Boing. null if nothing is in range.
        /// </summary>
        public InteractableGameobject InteractableGoInRange
        {
            get { return interactableGoInRange; }
            set { interactableGoInRange = value; }
        }
        private InteractableGameobject interactableGoInRange;

        /// <summary>
        /// LifePoints of Boing. Set value to damage it. Example LifePoints-- or -= 2.
        /// </summary>
        public int LifePoints
        {
            get { return lifePoints; }

            set {
                lifePoints = value;

                if(lifePoints <= 0)
                {
                    lifePoints = 0;
                    Die();
                }

                else if (lifePoints > maxLifePoints)
                    lifePoints = maxLifePoints;

                // Update HUD.
                GameManager.instance.UpdateLifePoints(lifePoints);

                // TODO temprorary invulnerability.
            }
        }
        private int lifePoints; // max 3
        
        public bool IsBouncing
        {
            get { return boingAnimator.GetBool("Bouncing"); }
            set { boingAnimator.SetBool("Bouncing", value); }
        }

        private Animator boingAnimator;

        private ParticleSystem noteEmitter;

        #endregion

        #region Unity methods

        private void Awake()
        {
            boingAnimator = GetComponent<Animator>();
            noteEmitter = GetComponentInChildren<ParticleSystem>();

            interactableGoInRange = null;

            lifePoints = maxLifePoints;
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, bouncingEffectRadius);
        }

        #endregion
        
        #region Public methods

        /// <summary>
        /// Boing starts to bounce.
        /// </summary>
        public void Bounce()
        {
            boingAnimator.SetTrigger("Bounce");
            IsBouncing = true;

            InvokeRepeating("BounceEffectInRange", 0.0f, 0.5f);
        }

        /// <summary>
        /// Boing stops to bounce.
        /// </summary>
        public void StopBounce()
        {
            IsBouncing = false;

            CancelInvoke("BounceEffectInRange");
        }

        public void Attack()
        {
            boingAnimator.SetTrigger("Attack");

            // TODO activate attack hitbox.
        }

        #endregion

        #region Private methods

        // Search all mob in range of the effect and trigger bounce effect to them.
        private void BounceEffectInRange()
        {
            noteEmitter.Emit(1);

            Collider2D[] mobs = Physics2D.OverlapCircleAll(transform.position, bouncingEffectRadius, whatIsMobs);

            foreach (Collider2D c in mobs)
            {
                if (c.tag == "Player" || c.gameObject.name == "OtherColliders")
                    continue;

                Mob mob = c.gameObject.GetComponent<Mob>();
                mob.TriggerBounce();
            }
        }

        private void Die()
        {
            // TODO animation, sound, ...

            GameManager.instance.FadeInAndReload();

            // Stop the player and disable inputs interaction.
            GetComponent<Platformer2DUserControl>().enabled = false;
            GetComponent<PlatformerCharacter2D>().Move(0.0f, false, false);
        }

        #endregion
    }
}