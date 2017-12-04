using UnityEngine;

namespace KekeDreamLand
{
    /// <summary>
    /// The mob which have this script move along the platform. If he reach an extremity he stops, flips and restart to move.
    /// 
    /// Based on hitbox bounds.
    /// </summary>
    public class MoveAlongThePlatform : AIBehaviour
    {
        #region Inspector attributes

        public float speed = 1.0f;

        [Tooltip("Check that if you want that this mob start to facingRight.")]
        public bool facingRight;

        [Tooltip("Check that if you want that this mob collide with other mobs.")]
        public bool collideWithOthers = false;

        public LayerMask whatIsGround;

        #endregion

        #region Private attributes

        private BoxCollider2D hitbox;
        private Mob mobScript;

        // Direction to move.
        private float direction;

        // Ray information.
        private Vector3 rayOrigin;
        private float rayOriginX;
        private float rayLength = 0.2f;

        #endregion

        #region Unity methods

        private void Awake()
        {
            mobScript = GetComponent<Mob>();
            hitbox = GetComponent<BoxCollider2D>();

            direction = facingRight ? 1.0f : -1.0f;
            // Flip the sprite if necessary.
            if (direction > 0)
                mobScript.FlipSprite();
        }

        private void FixedUpdate()
        {
            CheckPlatformExtremity();

            CheckWalls();

            Move();
        }

        #endregion

        #region Private methods

        private void CheckPlatformExtremity()
        {
            // Determine ray origin.
            if (direction > 0)
                rayOriginX = hitbox.bounds.max.x;
            else
                rayOriginX = hitbox.bounds.min.x;

            rayOrigin = new Vector3(rayOriginX, hitbox.bounds.min.y - 0.01f, hitbox.bounds.min.z);

            //Debug.DrawRay(rayOrigin, Vector2.down * rayLength, Color.red, 0.1f);

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, rayLength);

            // Change direction of the mob
            if (!hit || hit.collider && hit.collider.tag == "OutOfBound")
            {
                mobScript.FlipSprite();
                direction *= -1;
            }
        }

        private void CheckWalls()
        {
            if (direction > 0)
                rayOriginX = hitbox.bounds.max.x;
            else
                rayOriginX = hitbox.bounds.min.x;

            rayOrigin = new Vector3(rayOriginX, hitbox.bounds.center.y, hitbox.bounds.min.z);

            //Debug.DrawRay(rayOrigin, Vector2.left * rayLength * direction, Color.yellow, 0.1f);

            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.left * direction, rayLength, whatIsGround);

            // Change direction of the mob if it hits a wall.
            if (hit.collider != null && hit.collider.tag != "Interactable")
            {
                mobScript.FlipSprite();
                direction *= -1;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!collideWithOthers)
                return;

            if(collision.tag == "Ennemy")
            {
                mobScript.FlipSprite();
                direction *= -1;
            }
        }

        private void Move()
        {
            transform.Translate(Vector3.right * direction * speed * Time.deltaTime);
        }

        public override void SetupAI() {
            // Reset position ?
        }

        #endregion
    }
}