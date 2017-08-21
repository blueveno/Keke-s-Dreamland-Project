using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KekeDreamLand
{
    /// <summary>
    /// Dodo woodcutter shoot sawblade with him launcher. Stop it if he bounces.
    /// </summary>
    public class DodoWoodcutter : AIBehaviour {

        #region Inspector attributes

        public GameObject SawBladePrefab;
        public float timeToSpawn;
        public float shootForce;

        #endregion

        #region Private attributes

        private Transform sawBladeLauncher;

        #endregion

        #region Unity methods

        private void Awake()
        {
            sawBladeLauncher = transform.Find("Sawblade_launcher");
        }

        // Stop shooting blade.
        private void OnDisable()
        {
            CancelInvoke("ShootBlade");
        }

        // Restart shooting blade immediatly.
        private void OnEnable()
        {
            InvokeRepeating("ShootBlade", 0.0f, timeToSpawn);
        }

        #endregion

        #region Public methods

        #endregion

        #region Private methods 

        private void ShootBlade()
        {
            GameObject sawBlade = Instantiate(SawBladePrefab, sawBladeLauncher.position, Quaternion.identity);
            sawBlade.GetComponent<Rigidbody2D>().AddForce((Vector2.left + Vector2.up) * shootForce);
        }

        #endregion
    }
}