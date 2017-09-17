using UnityEngine;

namespace KekeDreamLand
{
    /// <summary>
    /// AI of a Squirrel. Throw a nut to Boing every 3 seconds.
    /// </summary>
    public class SquirrelAI : AIBehaviour
    {
        #region Inspector attributes

        public GameObject nutPrefab;
        
        public float throwingForce = 200.0f;

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

        #endregion

        #region Private methods 

        /// <summary>
        /// Animation Event triggered when the squirrel throw a nut.
        /// throw a projectile to the player (rectilinear trajectory).
        /// </summary>
        public void ThrowProjectile()
        {
            if (!enabled || target == null)
                return;

            // TODO activate only when the squirrel is on the same area that the player and at correct distance (sight view ?).
            if(Vector3.Distance(transform.position, target.transform.position) < 15.0f)
            {
                GameObject projectile = Instantiate(nutPrefab, throwingStartPoint.transform.position, Quaternion.identity);
                
                // Determine direction where to throw and normalized it.
                Vector2 dir = (target.transform.position - projectile.transform.position).normalized;

                // Throw the projectile.
                projectile.GetComponent<Rigidbody2D>().AddForce(dir * throwingForce);
            }
        }

        #endregion
    }
}