using System;
using UnityEngine;

namespace UnityStandardAssets._2D
{
    public class Camera2DFollow : MonoBehaviour
    {
        public Transform target;
        public float damping = 1;
        public float lookAheadFactor = 3;
        public float lookAheadReturnSpeed = 0.5f;
        public float lookAheadMoveThreshold = 0.1f;

        private float m_OffsetZ;
        private Vector3 m_LastTargetPosition;
        private Vector3 m_CurrentVelocity;
        private Vector3 m_LookAheadPos;
        
        // TODO : put limit on level manager.
        // Add by Bib' 13/08/17 - Level boundaries
        [Header("Level bounds :")]
        public bool displayBoundaries = true;
        public Bounds bounds;

        private float cameraMinX;
        private float cameraMaxX;
        private float cameraMinY;
        private float cameraMaxY;

        // Use this for initialization
        private void Start()
        {
            m_LastTargetPosition = target.position;
            m_OffsetZ = (transform.position - target.position).z;
            transform.parent = null;

            // Add by Bib'
            float cameraSizeY = Camera.main.orthographicSize;
            float cameraSizeX = cameraSizeY * Screen.width / Screen.height;

            cameraMinX = bounds.min.x + cameraSizeX;
            cameraMaxX = bounds.max.x - cameraSizeX;

            cameraMinY = bounds.min.y + cameraSizeY;
            cameraMaxY = bounds.max.y - cameraSizeY;
        }


        // Update is called once per frame
        private void Update()
        {
            // only update lookahead pos if accelerating or changed direction
            float xMoveDelta = (target.position - m_LastTargetPosition).x;

            bool updateLookAheadTarget = Mathf.Abs(xMoveDelta) > lookAheadMoveThreshold;

            if (updateLookAheadTarget)
            {
                m_LookAheadPos = lookAheadFactor*Vector3.right*Mathf.Sign(xMoveDelta);
            }
            else
            {
                m_LookAheadPos = Vector3.MoveTowards(m_LookAheadPos, Vector3.zero, Time.deltaTime*lookAheadReturnSpeed);
            }

            Vector3 aheadTargetPos = target.position + m_LookAheadPos + Vector3.forward*m_OffsetZ;
            Vector3 newPos = Vector3.SmoothDamp(transform.position, aheadTargetPos, ref m_CurrentVelocity, damping);

            newPos.x = Mathf.Clamp(newPos.x, cameraMinX, cameraMaxX);
            newPos.y = Mathf.Clamp(newPos.y, cameraMinY, cameraMaxY);

            transform.position = newPos;

            m_LastTargetPosition = target.position;
        }
        
        private void OnDrawGizmos()
        {
            DisplayLevelBoundaries();
        }

        // Display boundaries of the level on the editor only.
        private void DisplayLevelBoundaries()
        {
            if(displayBoundaries)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(bounds.center, bounds.extents*2);
            }
        }
    }
}
