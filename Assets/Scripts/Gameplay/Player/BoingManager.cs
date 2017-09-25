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

        [Header("Bouncing system")]
        public float bouncingEffectRadius = 2.5f;
        [SerializeField] private LayerMask whatIsMobs;

        [Header("Attack system")]
        public float timeBetweenAttack = 0.5f;
        public Vector2 attackHitbox;
        [Space]
        public bool debugAttackHitbox = true;
        public Color attackHitboxColor;

        [Header("Audio")]
        public AudioClip[] clips;

        #endregion

        #region Private attributes

        private Animator boingAnimator;
        private ParticleSystem noteEmitter;
        private new AudioSource audio;

        /// <summary>
        /// Current interactable gameobject in range of Boing. null if nothing is in range.
        /// </summary>
        public InteractableGameobject InteractableGoInRange { get; set; }

        /// <summary>
        /// Current life points of Boing. Set value to damage it or heal it. Example LifePoints-- or -= 2.
        /// </summary>
        public int LifePoints
        {
            get { return lifePoints; }

            set {
                // Boing is healing.
                if (value > lifePoints)
                {
                    lifePoints = Mathf.Min(value, maxLifePoints);
                }

                // Boing is taking damage.
                else
                {
                    // No taking damage while Boing is invulnerable.
                    if (isInvulnerable)
                        return;

                    // Boing is damaged and no lifepoints remind.
                    if (value <= 0)
                    {
                        lifePoints = 0;
                        
                        // Destroy Boing immediatly. TODO trigger particle effect but disble sprite then destroy it.
                        Die(0);
                    }

                    // Boing is damaged but alive.
                    else
                    {
                        // Trigger temporary invulnerability if Boing has taken a damage.
                        StartCoroutine(Invulnerability());

                        lifePoints = value;
                    }
                }
                
                // Update HUD.
                GameManager.instance.CurrentLevel.UpdateLifePoints(lifePoints);
            }
        }
        private int lifePoints; // current life points.
        
        /// <summary>
        /// Set boing invulenrable or check if he's invulnerable.
        /// </summary>
        public bool IsInvulnerable
        {
            get { return isInvulnerable; }
            set { isInvulnerable = value; }
        }
        private bool isInvulnerable = false;

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

            InteractableGoInRange = null;
        }

        private void Start()
        {
            LifePoints = maxLifePoints;
        }

        private void OnDrawGizmosSelected()
        {
            // Debug bouncing range circle.
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, bouncingEffectRadius);

            // Debug attack hitbox.
            if (debugAttackHitbox)
            {
                Gizmos.color = attackHitboxColor;
                // Attack hitbox is in front of Boing and depends from its orientation.
                Vector2 pos = new Vector2(transform.position.x + transform.localScale.x * (attackHitbox.x / 2), transform.position.y + 0.05f);
                Gizmos.DrawCube(pos, attackHitbox);
            }
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

            /*
            audio.clip = clips[0];
            audio.Play();
            */

            yield return new WaitForSeconds(0.25f);

            // Positionnate and create attack hitbox.
            Vector2 pos = new Vector2(transform.position.x + transform.localScale.x * (attackHitbox.x / 2), transform.position.y + 0.05f);
            Collider2D[] hits = Physics2D.OverlapBoxAll(pos, attackHitbox, 0.0f, whatIsMobs);

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
                if(mob)
                    mob.TriggerBounce();
            }
        }

        // Shake the sprite.
        private IEnumerator Invulnerability()
        {
            isInvulnerable = true;

            int i = 0;
            int blinkCount = 12;

            SpriteRenderer sprite = GetComponent<SpriteRenderer>();

            while (i < blinkCount)
            {
                // Change color of Boing.
                if(i % 2 == 0)
                    sprite.color = Color.grey;
                else
                    sprite.color = Color.white;

                yield return new WaitForSeconds(invulnerabilityDuration / blinkCount);
                i++;
            }
            
            if (!GameManager.instance.CurrentLevel.IsInternalTransition)
                isInvulnerable = false;

            // Reset color of the sprite.
            sprite.color = Color.white;
        }

        /// <summary>
        /// Trigger Boing Death. Feedback then destroy and respawn/reload.
        /// </summary>
        /// <param name="destroyDelay"></param>
        public void Die(float destroyDelay)
        {
            // TODO animation, sound, particle effect, ...
            
            // Stop Boing velocity.
            GetComponent<InputManager>().StopBoing();
            
            // Destroy Boing.
            Destroy(gameObject, destroyDelay);

            // TODO foreach item of the list, restore it on the level, remove it from HUD and clear items list.

            // Reload scene or respawn.
            GameManager.instance.BoingDie();
        }

        #endregion
    }
}