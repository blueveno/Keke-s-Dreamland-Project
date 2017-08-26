using System.Collections.Generic;
using UnityEngine;

namespace KekeDreamLand
{
    public enum PatrolType
    {
        // Flip and going back when reach an extremity of the patrol path.
        GOING_AND_COMING,

        // Go to the first patrol point when it reach the end.
        CYCLIC,

        // Teleport to the first patrol point when it reach the end.
        WARP
    }

    public class Patrol : AIBehaviour
    {
        #region Inspector attributes
        
        public float patrolSpeed = 1.0f;
        public bool displayPatrolPath = true;
        public PatrolType patrolType;
        public List<Vector3> patrolPoints;

        #endregion

        #region Private attributes

        private Mob mobScript;

        private Vector3 initialPosition;
        private Vector3 nextPatrolPoint;
        private int nextPatrolPointIndex;

        private bool reverseDirection = false;

        #endregion

        #region Unity methods

        // Use this for initialization
        void Awake()
        {
            mobScript = GetComponent<Mob>();

            initialPosition = transform.position;
            nextPatrolPointIndex = 1;

            DetermineNextPatrolPoint();
        }

        // Update is called once per frame
        void Update()
        {
            float distance = Vector3.Distance(transform.position, initialPosition + nextPatrolPoint);

            if (distance > Mathf.Epsilon)
                transform.position = Vector3.MoveTowards(transform.position, initialPosition + nextPatrolPoint, patrolSpeed * Time.deltaTime);

            else
                DetermineNextPatrolPoint();
        }

        #endregion

        #region Patrol methods

        // Determine the next patrol point to reach depending the type of patrol choosen.
        private void DetermineNextPatrolPoint()
        {
            switch(patrolType)
            {
                case PatrolType.GOING_AND_COMING:

                    GoingAndComingPatrol(); break;

                case PatrolType.CYCLIC:

                    CyclicPatrol(); break;

                case PatrolType.WARP:

                    WarpPatrol(); break;
            }
        }
        
        private void GoingAndComingPatrol()
        {
            if (nextPatrolPointIndex == patrolPoints.Count)
            {
                reverseDirection = true;
                nextPatrolPointIndex = patrolPoints.Count - 1;

                mobScript.FlipSprite();
            }
            else if (nextPatrolPointIndex == -1)
            {
                reverseDirection = false;
                nextPatrolPointIndex = 1;

                mobScript.FlipSprite();
            }

            if (reverseDirection)
                nextPatrolPoint = patrolPoints[nextPatrolPointIndex--];
            else
                nextPatrolPoint = patrolPoints[nextPatrolPointIndex++];
        }
        
        private void CyclicPatrol()
        {
            if (nextPatrolPointIndex == patrolPoints.Count)
            {
                nextPatrolPointIndex = 0;
            }

            nextPatrolPoint = patrolPoints[nextPatrolPointIndex++];
        }

        private void WarpPatrol()
        {
            if (nextPatrolPointIndex == patrolPoints.Count)
            {
                transform.position = initialPosition + patrolPoints[0];
                nextPatrolPointIndex = 1;
            }

            nextPatrolPoint = patrolPoints[nextPatrolPointIndex++];
        }

        #endregion

        #region Level design help

        private void OnDrawGizmosSelected()
        {
            if (displayPatrolPath)
            {
                DisplayPatrolPath();
            }
        }

        private void DisplayPatrolPath()
        {
            Gizmos.color = Color.blue;

            Vector3 mobPosition = Vector3.zero;

            if (Application.isPlaying)
                mobPosition = initialPosition;
            else
                mobPosition = transform.position;

            foreach (Vector3 pos in patrolPoints)
            {
                Gizmos.DrawSphere(mobPosition + pos, 0.1f);
            }
        }

        #endregion
    }
}