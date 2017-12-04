using UnityEngine;

namespace KekeDreamLand
{
    /// <summary>
    /// AI of a Squirrel. Throw a nut to Boing each time that this script receive animation event.
    /// </summary>
    public class SquirrelAI : AIBehaviour
    {
        #region Inspector attributes

        [Header("Throw information")]
        public GameObject nutPrefab;
        public float throwingForce = 1f;
        public LayerMask targetLayer;

        #endregion

        #region Private attributes

        private Animator anim;

        private GameObject target;
        private GameObject throwingStartPoint;

        #endregion

        #region Unity methods

        private void Awake()
        {
            anim = GetComponent<Animator>();
            throwingStartPoint = transform.GetChild(0).gameObject;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "Player")
            {
                anim.SetBool("Throwing", true);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "Player")
            {
                anim.SetBool("Throwing", false);
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Animation Event triggered when the squirrel throw a nut.
        /// throw a projectile to the player (rectilinear trajectory).
        /// </summary>
        public void ThrowProjectile()
        {
            if (!enabled || target == null)
                return;
            
            GameObject projectile = Instantiate(nutPrefab, throwingStartPoint.transform.position, Quaternion.identity);

            // Determine direction where to throw and normalized it.
            Vector2 dir = (target.transform.position - projectile.transform.position).normalized;

            // Throw the projectile.
            projectile.GetComponent<Rigidbody2D>().velocity = dir * throwingForce;
        }

        public override void SetupAI()
        {
            target = GameObject.FindGameObjectWithTag("Player");

            anim.SetTrigger("Reset");
        }

        #endregion
    }
}