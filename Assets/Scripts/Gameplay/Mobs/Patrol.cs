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
        public List<Vector3> patrolPoints = new List<Vector3>();

        #endregion

        #region Private attributes

        private Mob mobScript;

        protected Vector3 initialPosition;
        protected Vector3 nextPatrolPoint;
        private int nextPatrolPointIndex;

        private bool reverseDirection = false;

        #endregion

        #region Unity methods

        // Use this for initialization
        protected void Awake()
        {
            initialPosition = transform.position;
        }

        // Update is called once per frame
        protected void Update()
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
        protected void DetermineNextPatrolPoint()
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

        protected void GoingAndComingPatrol()
        {
            if (nextPatrolPointIndex == patrolPoints.Count)
            {
                reverseDirection = true;
                nextPatrolPointIndex = patrolPoints.Count - 1;
            }
            else if (nextPatrolPointIndex == -1)
            {
                reverseDirection = false;
                nextPatrolPointIndex = 1;
            }

            if (reverseDirection)
                nextPatrolPoint = patrolPoints[nextPatrolPointIndex--];
            else
                nextPatrolPoint = patrolPoints[nextPatrolPointIndex++];
        }

        protected void CyclicPatrol()
        {
            if (nextPatrolPointIndex == patrolPoints.Count)
            {
                nextPatrolPointIndex = 0;
            }

            nextPatrolPoint = patrolPoints[nextPatrolPointIndex++];
        }

        protected void WarpPatrol()
        {
            if (nextPatrolPointIndex == patrolPoints.Count)
            {
                transform.position = initialPosition + patrolPoints[0];
                nextPatrolPointIndex = 1;
            }

            nextPatrolPoint = patrolPoints[nextPatrolPointIndex++];
        }

        #endregion

        public override void SetupAI()
        {
            transform.position = initialPosition;
            nextPatrolPointIndex = 1;

            DetermineNextPatrolPoint();
        }

        #region Level design help

        protected void OnDrawGizmosSelected()
        {
            if (displayPatrolPath)
            {
                DisplayPatrolPath();
            }
        }

        protected void DisplayPatrolPath()
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