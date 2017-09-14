using UnityEngine;

namespace KekeDreamLand
{
    /// <summary>
    /// The mushroom jumps when Boing jumps.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class Mushroom : AIBehaviour, IObserver
    {
        #region Inspector attributes
        [Header("Mushroom AI configuration :")]
        public float jumpForce = 20.0f;

        #endregion

        #region Private attributes

        private Rigidbody2D rgbd;

        #endregion

        #region Unity methods
        
        protected void Awake()
        {
            rgbd = GetComponent<Rigidbody2D>();

            // Observe Boing user control.
            InputManager userController = GameObject.FindGameObjectWithTag("Player").GetComponent<InputManager>();
            userController.AddObserver(this);
        }

        #endregion

        /// <summary>
        /// Notify that Boing has jump.
        /// </summary>
        public void NotifyJump()
        {
            // Prevent propulsion and message from observer when ai is disabled.
            if(rgbd.velocity.y == 0 && enabled)
                Jump();
                
        }

        private void Jump()
        {
            rgbd.AddForce(Vector2.up * jumpForce);
        }

        private void OnDestroy()
        {
            // Remove input observer when mushroom dies.
            GameObject player = GameObject.FindGameObjectWithTag("Player");

            if(player)
                player.GetComponent<InputManager>().RemoveObserver(this);
        }
    }
}