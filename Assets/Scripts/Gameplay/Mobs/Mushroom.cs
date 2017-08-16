using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KekeDreamLand
{
    /// <summary>
    /// The mushroom jumps when Boing jumps.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class Mushroom : Mob, Observer
    {
        #region Inspector attributes
        [Header("Mushroom configuration :")]
        public float jumpForce = 20.0f;

        #endregion

        #region Private attributes

        private Rigidbody2D rgbd;

        #endregion

        #region Unity methods

        private void Awake()
        {
            rgbd = GetComponent<Rigidbody2D>();
        }

        private void Start()
        {
            // Observe Boing user control.
            Platformer2DUserControl userController = GameObject.FindGameObjectWithTag("Player").GetComponent<Platformer2DUserControl>();
            userController.AddObserver(this);
        }

        #endregion

        /// <summary>
        /// Notify that Boing has jump (TODO transform to has done an action (create an enum actionType and treat them).
        /// </summary>
        public void NotifyJump()
        {
            Jump();
        }

        private void Jump()
        {
            rgbd.AddForce(Vector2.up * jumpForce);
        }
    }
}