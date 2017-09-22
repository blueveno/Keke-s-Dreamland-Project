using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KekeDreamLand
{
    [System.Serializable]
    public class Platform
    {
        public GameObject gameObject = null;
        public int nextPathlPointIndex;
        public Vector3 nextPathPoint;

        public Platform()
        {
            nextPathlPointIndex = 1;
            nextPathPoint = Vector3.zero;
        }
    }

    public class MovingPlatforms : MonoBehaviour
    {
        #region Inspector attributes

        [Header("Moving platform patern")]
        [Tooltip("Speed of all moving platforms.")]
        public float moveSpeed = 1.0f;
        public PatrolType movementType;
        [Space]
        public bool displayPath = true;
        [Tooltip("Path followed by all moving platforms.")]
        public List<Vector3> pathPoints = new List<Vector3>();
        #endregion

        #region Private attributes
        
        private Platform[] platforms;

        // For going and coming patern.
        private bool reverseDirection = false;

        #endregion

        #region Unity methods

        // Use this for initialization
        private void Awake()
        {
            platforms = new Platform[transform.childCount];

            for (int i = 0; i < platforms.Length; i++)
            {
                platforms[i] = new Platform
                {
                    gameObject = transform.GetChild(i).gameObject
                };

                DetermineNextPatrolPoint(platforms[i]);
            }
        }

        // Update is called once per frame
        private void FixedUpdate()
        {
            foreach (Platform p in platforms)
            {
                float distance = Vector3.Distance(p.gameObject.transform.position, transform.position + p.nextPathPoint);

                if (distance > Mathf.Epsilon)
                    p.gameObject.transform.position = Vector3.MoveTowards(p.gameObject.transform.position, transform.position + p.nextPathPoint, moveSpeed * Time.deltaTime);
                else
                    DetermineNextPatrolPoint(p);
            }
        }

        #endregion

        #region Patrol methods

        // Determine the next patrol point to reach depending the type of patrol choosen.
        private void DetermineNextPatrolPoint(Platform p)
        {
            switch (movementType)
            {
                case PatrolType.GOING_AND_COMING:

                    GoingAndComingPatern(p); break;

                case PatrolType.CYCLIC:

                    CyclicPatern(p); break;

                case PatrolType.WARP:

                    WarpPatern(p); break;
            }
        }

        private void GoingAndComingPatern(Platform p)
        {
            if (p.nextPathlPointIndex == pathPoints.Count)
            {
                reverseDirection = true;
                p.nextPathlPointIndex = pathPoints.Count - 1;
            }
            else if (p.nextPathlPointIndex == -1)
            {
                reverseDirection = false;
                p.nextPathlPointIndex = 1;
            }

            if (reverseDirection)
                p.nextPathPoint = pathPoints[p.nextPathlPointIndex--];
            else
                p.nextPathPoint = pathPoints[p.nextPathlPointIndex++];
        }

        private void CyclicPatern(Platform p)
        {
            if (p.nextPathlPointIndex == pathPoints.Count)
            {
                p.nextPathlPointIndex = 0;
            }

            p.nextPathPoint = pathPoints[p.nextPathlPointIndex++];
        }

        private void WarpPatern(Platform p)
        {
            if (p.nextPathlPointIndex == pathPoints.Count)
            {
                p.gameObject.transform.position = transform.position + pathPoints[0];
                p.nextPathlPointIndex = 1;
            }

            p.nextPathPoint = pathPoints[p.nextPathlPointIndex++];
        }

        #endregion

        #region Level design help

        private void OnDrawGizmosSelected()
        {
            if (displayPath)
            {
                DisplayPath();
            }
        }

        private void DisplayPath()
        {
            Gizmos.color = Color.green;

            Vector3 spawnPosition = transform.position;

            foreach (Vector3 pos in pathPoints)
            {
                Gizmos.DrawSphere(spawnPosition + pos, 0.1f);
            }
        }

        #endregion
    }
}
