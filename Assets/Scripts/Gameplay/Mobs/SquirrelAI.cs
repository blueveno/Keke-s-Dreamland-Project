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
        public float throwingForce = 200.0f;
        public LayerMask targetLayer;

        [Header("Sight")]
        public bool debugSight = true;
        public Transform sightBoxCenter;
        public Vector2 sightBox = new Vector2(4.0f, 4.0f);

        #endregion

        #region Private attributes

        private GameObject target;
        private GameObject throwingStartPoint;

        #endregion

        #region Unity methods

        private void Awake()
        {
            // Track player.
            target = GameObject.FindGameObjectWithTag("Player");
            
            throwingStartPoint = transform.GetChild(0).gameObject;
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

            Collider2D[] inArea = Physics2D.OverlapBoxAll(sightBoxCenter.position, sightBox, 0.0f, targetLayer);

            foreach(Collider2D c in inArea)
            {
                if(c.tag == "Player")
                {
                    GameObject projectile = Instantiate(nutPrefab, throwingStartPoint.transform.position, Quaternion.identity);

                    // Determine direction where to throw and normalized it.
                    Vector2 dir = (target.transform.position - projectile.transform.position).normalized;

                    // Throw the projectile.
                    projectile.GetComponent<Rigidbody2D>().AddForce(dir * throwingForce);
                }
            }
        }

        #endregion

        #region Private methods 

        private void OnDrawGizmos()
        {
            if (!debugSight)
                return;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(sightBoxCenter.position, sightBox);
        }

        #endregion
    }
}