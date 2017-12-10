using System.Collections;
using UnityEngine;

namespace KekeDreamLand
{
    /// <summary>
    /// Attach this to any enemy in the game. Inherit if the mob need a specialized behaviour.
    /// </summary>
    public class Mob : MonoBehaviour, IAction {

        #region Inspector Attributes
        
        [Header("Damage system")]
        [Tooltip("Damage that the mob deals to Boing when it collide with it.")]
        public int mobDamage = 1;

        [Header("Life system")]
        public bool invincible;
        public int maxLifePoints = 1;

        [Header("Bounce behaviour")]
        public bool canBounce = false;
        public float BounceEffectDuration = 2.0f;

        [Header("Death Particles")]
        public GameObject deathParticleEmitter;

        #endregion

        #region Private attributes

        protected bool isBouncing;
        private float timer = 0.0f; // Timer of bouncing.

        protected Animator animator;
        private GameObject noteEmitter;
        
        /// <summary>
        /// LifePoints of this mob. Set value to damage it. Example LifePoints-- or -= 2.
        /// </summary>
        public int LifePoints
        {
            get { return lifePoints; }

            set
            {
                // No taking damage if invincible.
                if (invincible)
                {
                    Debug.LogWarning(name + " is invincible !");
                    return;
                }

                // Mob dies.
                if (value <= 0)
                {
                    lifePoints = 0;
                    Die();
                    return;
                }

                // Heal the mob.
                if (value > lifePoints)
                    lifePoints = Mathf.Min(value, maxLifePoints);

                // Shake sprite when mob is damaged but don't die.
                else
                {
                    lifePoints = value;
                    StartCoroutine(ShakeSprite());
                }
            }
        }
        private int lifePoints;

        private Vector2 initialPosition;

        #endregion

        #region Unity methods

        protected virtual void Awake()
        {
            animator = GetComponent<Animator>();

            if(canBounce)
                noteEmitter = transform.Find("NoteEmitter").gameObject;

            initialPosition = transform.position;

            SetupMob();
        }

        public void SetupMob()
        {
            // Reset initial position.
            transform.position = initialPosition;
            // Full heal.
            lifePoints = maxLifePoints;

            // interrupt bouncing.
            if (isBouncing)
            {
                StopAllCoroutines();
                StopBouncing();
            }

            // Reset AI.
            ToggleAIBehaviours(true);
        }

        protected void OnTriggerEnter2D(Collider2D collision)
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
                Debug.LogWarning(name + " can't bounce !");

            // TODO feedback when not affected.
        }

        #endregion

        #region Private methods

        protected void DealDamageToPlayer(GameObject player)
        {
            player.GetComponent<BoingManager>().LifePoints -= mobDamage;
        }
        
        private void Die()
        {
            // TODO pool for death particle systems.
            if (deathParticleEmitter)
            {
                Vector3 pos = transform.position;
                pos.y -= 0.1f;

                GameObject deathParticleSystem =  Instantiate(deathParticleEmitter, pos, Quaternion.identity);
                deathParticleSystem.GetComponent<ParticleSystem>().Emit(2);
                Destroy(deathParticleSystem, 1.0f);
            }

            // Disable temporary him.
            gameObject.SetActive(false);

            StopAllCoroutines();

            // Register that this mob has been killed.
            GameManager.instance.levelMgr.RegisterAction(this);
        }

        private void Bounce()
        {
            if (!isBouncing)
                StartCoroutine(BounceWhileAffected());

            // Reset cooldown of bouncing.
            else
                timer = BounceEffectDuration;
        }

        private IEnumerator BounceWhileAffected()
        {
            isBouncing = true;
            
            // Trigger bouncing effect.
            ToggleAIBehaviours(false);
            animator.SetBool("Bouncing", isBouncing);
            noteEmitter.SetActive(isBouncing);

            BoingManager player = GameObject.FindGameObjectWithTag("Player").GetComponent<BoingManager>();
            
            // Don't do anything while Boing bounce.
            yield return new WaitWhile(() => (player != null && player.IsBouncing));

            // Cooldown. Reset if Boing bounce one more time.
            timer = BounceEffectDuration;
            while(timer >= 0.0f)
            {
                yield return new WaitForSeconds(0.5f);
                timer -= 0.5f;
            }
            
            StopBouncing();
        }

        // Stop bouncing effect.
        private void StopBouncing()
        {
            isBouncing = false;

            if(enabled)
                animator.SetBool("Bouncing", isBouncing);

            noteEmitter.SetActive(isBouncing);
            ToggleAIBehaviours(true);
        }

        private void ToggleAIBehaviours(bool enabled)
        {
            // Get all AIs on the mob.
            AIBehaviour[] ais = GetComponents<AIBehaviour>();

            foreach(AIBehaviour ai in ais)
            {
                ai.enabled = enabled;

                // Debug ai activation or desactivation.
                //Debug.Log(ai.name + " : " + enabled);
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
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Flip the sprite verticaly.
        /// </summary>
        public void FlipSprite()
        {
            // Multiply the mob's x local scale by -1.
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }

        /// <summary>
        /// Message received when this mob is pecked by Boing.
        /// </summary>
        public void OnPeck()
        {
            LifePoints--;
        }

        // Destroy this mob from actions when a checkpoint is reached.
        public void DeleteAction()
        {
            Destroy(gameObject);
        }

        // Reset this mob when player dies.
        public void CancelAction()
        {
            SetupMob();
            gameObject.SetActive(true);
        }

        #endregion
    }
}