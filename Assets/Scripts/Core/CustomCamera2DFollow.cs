using System;
using UnityEngine;

namespace KekeDreamLand
{
    public class CustomCamera2DFollow : MonoBehaviour
    {
        #region Initial attributes
        public Transform target;
        public float damping = 1;
        public float lookAheadFactor = 3;
        public float lookAheadReturnSpeed = 0.5f;
        public float lookAheadMoveThreshold = 0.1f;

        private float m_OffsetZ;
        private Vector3 m_LastTargetPosition;
        private Vector3 m_CurrentVelocity;
        private Vector3 m_LookAheadPos;
        #endregion

        #region Camera follow limit
        // TODO : put limit on level manager.
        // Add by Bib' 13/08/17 - Level boundaries
        private LevelEditor currentArea;
        public LevelEditor CurrentArea
        {
            get
            {
                return currentArea;
            }

            set
            {
                currentArea = value;

                // Change limit and repositionate camera.
                SetupCameraLimit();
                SetupCameraPosition();
            }
        }

        private float cameraMinX;
        private float cameraMaxX;
        private float cameraMinY;
        private float cameraMaxY;

        #endregion

        // Use this for initialization
        private void Start()
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;

            if (!target)
            {
                Debug.LogWarning("Boing is not present in the level !!! \nPlace it into an area in the \"Level/character\" section.");
                return;
            }

            m_LastTargetPosition = target.position;
            m_OffsetZ = (transform.position - target.position).z;
            transform.parent = null;
            
            // Recover current area.
            currentArea = target.transform.parent.parent.parent.GetComponent<LevelEditor>();

            SetupCameraLimit();
            
            SetupCameraPosition();
        }

        private void SetupCameraPosition()
        {
            Vector3 newPos = Vector3.zero;

            // Limit the camera.
            newPos.x = Mathf.Clamp(target.position.x, cameraMinX, cameraMaxX);
            newPos.y = Mathf.Clamp(target.position.y, cameraMinY, cameraMaxY);

            transform.position = newPos;
        }

        // Setup camera follow limit.
        private void SetupCameraLimit()
        {
            float cameraSizeY = Camera.main.orthographicSize;
            float cameraSizeX = cameraSizeY * Screen.width / Screen.height;

            cameraMinX = currentArea.transform.position.x + cameraSizeX;
            cameraMaxX = currentArea.transform.position.x + currentArea.level.column - cameraSizeX;

            cameraMinY = currentArea.transform.position.y + cameraSizeY;
            cameraMaxY = currentArea.transform.position.y + currentArea.level.raw - cameraSizeY;
        }
        
        // Update is called once per frame
        private void Update()
        {
            if (target == null)
                return;

            // Only update lookahead pos if accelerating or changed direction
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
            
            // Restrict the camera in the boundaries of the current area.
            newPos.x = Mathf.Clamp(newPos.x, cameraMinX, cameraMaxX);
            newPos.y = Mathf.Clamp(newPos.y, cameraMinY, cameraMaxY);

            transform.position = newPos;

            m_LastTargetPosition = target.position;
        }
    }
}
