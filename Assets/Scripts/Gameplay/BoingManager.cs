﻿using System.Collections;
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
        [SerializeField] private LayerMask whatIsMobs;

        [Header("Attack")]
        public float timeBetweenAttack = 0.5f;
        public Vector2 attackBox;

        #endregion

        #region Private attributes

        private Animator boingAnimator;
        private ParticleSystem noteEmitter;

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
                GameManager.instance.CurrentLevel.UpdateLifePoints(lifePoints);

                // TODO temprorary invulnerability.
            }
        }
        private int lifePoints; // max 3
        
        /// <summary>
        /// Return true if Boing is actually bouncing.
        /// </summary>
        public bool IsBouncing
        {
            get { return boingAnimator.GetBool("Bouncing"); }
            set { boingAnimator.SetBool("Bouncing", value); }
        }

        // Attack attributes
        private bool isAttacking;

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
            // Debug bouncing range circle.
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, bouncingEffectRadius);

            // Debug attack hitbox.
            Gizmos.color = Color.red;
            Vector2 pos = new Vector2(transform.position.x + transform.localScale.x * 0.5f, transform.position.y + 0.05f);
            Gizmos.DrawCube(pos, attackBox);
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

        /// <summary>
        /// Trigger attack of Boing if it isn't already attacking.
        /// </summary>
        public void Attack()
        {
            if (!isAttacking)
            {
                StartCoroutine(AttackEffect());
            }
        }

        private IEnumerator AttackEffect()
        {
            isAttacking = true;

            boingAnimator.SetTrigger("Attack");

            yield return new WaitForSeconds(0.25f);

            // Positionnate and create attack hitbox.
            Vector2 pos = new Vector2(transform.position.x + transform.localScale.x * 0.5f, transform.position.y + 0.05f);
            Collider2D[] hits = Physics2D.OverlapBoxAll(pos, attackBox, 0.0f, whatIsMobs);

            // Attack concerns all mob touch by the ray.
            foreach (Collider2D hit in hits)
            {
                if (hit && hit.tag != "Player")
                {
                    Mob mob = hit.gameObject.GetComponent<Mob>();

                    // Damage mob hit.
                    if (mob)
                        mob.LifePoints--;
                }
            }

            yield return new WaitForSeconds(0.25f);

            isAttacking = false;
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

            GameManager.instance.TriggerFadeIn();

            // Stop Boing velocity.
            GetComponent<Platformer2DUserControl>().StopBoing();
        }

        #endregion
    }
}