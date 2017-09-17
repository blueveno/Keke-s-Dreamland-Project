using System.Collections;
using UnityEngine;

namespace KekeDreamLand
{
    public class PlatformerCharacter2D : MonoBehaviour
    {
        #region Inspector attributes

        [SerializeField] private float m_maxYSpeed = 4f;
        [SerializeField] private float m_MaxSpeed = 10f;                    // The fastest the player can travel in the x axis.
        [SerializeField] private float m_JumpForce = 400f;                  // Amount of force added when the player jumps.
        [SerializeField] private bool m_AirControl = false;                 // Whether or not a player can steer while jumping;
        [SerializeField] private LayerMask m_WhatIsGround;                  // A mask determining what is ground to the character

        #endregion

        #region Private attributes

        private Transform m_GroundCheck;    // A position marking where to check if the player is grounded.
        const float k_GroundedRadius = .12f; // Radius of the overlap circle to determine if grounded
        private bool m_Grounded;            // Whether or not the player is grounded.
        private Animator m_Anim;            // Reference to the player's animator component.

        private Rigidbody2D m_Rigidbody2D;
        private BoxCollider2D m_collider;
        private CircleCollider2D m_secondCollider;

        private bool m_FacingRight = true;  // For determining which way the player is currently facing.

        public bool IsGrounded
        {
            get { return m_Grounded; }
        }

        public float VSpeed
        {
            get { return m_Anim.GetFloat("vSpeed"); }
        }

        #endregion

        #region Unity methods

        private void Awake()
        {
            // Setting up references.
            m_GroundCheck = transform.Find("GroundCheck");
            m_Anim = GetComponent<Animator>();
            m_Rigidbody2D = GetComponent<Rigidbody2D>();
            m_collider = GetComponent<BoxCollider2D>();
            m_secondCollider = transform.GetChild(0).gameObject.GetComponent<CircleCollider2D>();
        }

        private void FixedUpdate()
        {
            m_Grounded = false;

            // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
            // This can be done using layers instead but Sample Assets will not overwrite your project settings.
            Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);

            for (int i = 0; i < colliders.Length; i++)
            {
                // Update by Bib' -> trigger colliders are not ground.
                if (colliders[i].gameObject != gameObject && !colliders[i].isTrigger)
                    m_Grounded = true;
            }
            m_Anim.SetBool("Ground", m_Grounded);

            // Set the vertical animation
            m_Anim.SetFloat("vSpeed", m_Rigidbody2D.velocity.y);

            if (m_Rigidbody2D.velocity.y >= m_maxYSpeed)
            {
                m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, m_maxYSpeed);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;

            if (m_GroundCheck != null)
                Gizmos.DrawSphere(m_GroundCheck.position, k_GroundedRadius);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Manage movement.
        /// </summary>
        /// <param name="move">horizontal movement</param>
        /// <param name="jump">is jump pressed</param>
        public void Move(float move, bool jump)
        {
            // Only control the player if grounded or airControl is turned on
            if (m_Grounded || m_AirControl)
            {
                // The Speed animator parameter is set to the absolute value of the horizontal input.
                m_Anim.SetFloat("Speed", Mathf.Abs(move));

                // Move the character
                m_Rigidbody2D.velocity = new Vector2(move*m_MaxSpeed, m_Rigidbody2D.velocity.y);

                // If the input is moving the player right and the player is facing left...
                if (move > 0 && !m_FacingRight)
                {
                    // ... flip the player.
                    Flip();
                }
                    // Otherwise if the input is moving the player left and the player is facing right...
                else if (move < 0 && m_FacingRight)
                {
                    // ... flip the player.
                    Flip();
                }
            }

            // If the player should jump...
            if (m_Grounded && jump && m_Anim.GetBool("Ground"))
            {
                // Add a vertical force to the player.
                m_Grounded = false;
                m_Anim.SetBool("Ground", false);
                
                m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));

                m_Anim.SetTrigger("Jump");
            }
        }

        /// <summary>
        /// Try to pass through a one sided platform.
        /// </summary>
        public void MoveDown()
        {
            Vector2 center = new Vector2((m_collider.bounds.min.x + m_collider.bounds.max.x) / 2, m_collider.bounds.min.y);
            RaycastHit2D hit = Physics2D.Raycast(center, Vector2.down, 0.25f, m_WhatIsGround);
            
            if (hit)
            {
                GameObject g = hit.collider.gameObject;
                
                if (g)
                {
                    PlatformEffector2D platform = g.GetComponent<PlatformEffector2D>();
                    if(platform)
                    {
                        Physics2D.IgnoreCollision(hit.collider, m_collider, true);
                        Physics2D.IgnoreCollision(hit.collider, m_secondCollider, true);

                        StartCoroutine(ResetIgnoringOfCollision(hit.collider));
                    }
                }
            }
        }

        #endregion

        #region Private methods

        private IEnumerator ResetIgnoringOfCollision(Collider2D platform)
        {
            yield return new WaitForSeconds(0.5f);

            Physics2D.IgnoreCollision(platform, m_collider, false);
            Physics2D.IgnoreCollision(platform, m_secondCollider, false);
        }

        private void Flip()
        {
            // Switch the way the player is labelled as facing.
            m_FacingRight = !m_FacingRight;

            // Multiply the player's x local scale by -1.
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }

        #endregion
    }
}
