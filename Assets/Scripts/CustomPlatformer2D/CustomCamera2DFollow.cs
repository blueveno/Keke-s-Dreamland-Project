using UnityEngine;

namespace KekeDreamLand
{
    public class CustomCamera2DFollow : MonoBehaviour
    {
        #region Initial attributes
        public Transform target;

        /*
        public float damping = 1;
        public float lookAheadFactor = 3;
        public float lookAheadReturnSpeed = 0.5f;
        public float lookAheadMoveThreshold = 0.1f;

        private Vector3 m_LastTargetPosition;
        private Vector3 m_CurrentVelocity;
        private Vector3 m_LookAheadPos;
        */

        private float m_OffsetZ;
        #endregion

        #region Camera follow restriction
        
        // Current area where Boing is.
        public AreaEditor CurrentArea
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
                FollowPlayer();
            }
        }
        private AreaEditor currentArea;

        // Camera limit X and Y.
        private float cameraMinX;
        private float cameraMaxX;
        private float cameraMinY;
        private float cameraMaxY;

        private bool lockVertical;
        private bool lockHorizontal;

        #endregion

        #region Unity methods

        // Use this for initialization
        private void Start()
        {
            // Search for player gameobject.
            target = GameObject.FindGameObjectWithTag("Player").transform;

            if (!target)
            {
                Debug.LogWarning("Boing is not present in the level !!! \nPlace it into an area in the \"Level/character\" section.");
                return;
            }

            // m_LastTargetPosition = target.position; // Used for smooth follow.

            m_OffsetZ = (transform.position - target.position).z;
            transform.parent = null;
            
            // Recover current area.
            CurrentArea = target.transform.parent.parent.parent.GetComponent<AreaEditor>();
        }
        
        // Update is called once per frame
        private void LateUpdate()
        {
            if (target == null)
                return;

            FollowPlayer();
        }

        #endregion

        #region Camera follow player and is restricted by area boundaries.

        // Setup camera follow limit.
        private void SetupCameraLimit()
        {
            float cameraSizeY = Camera.main.orthographicSize;
            float cameraSizeX = cameraSizeY * Screen.width / Screen.height;

            cameraMinX = currentArea.transform.position.x + cameraSizeX;
            cameraMaxX = currentArea.transform.position.x + currentArea.area.column - cameraSizeX;

            cameraMinY = currentArea.transform.position.y + cameraSizeY;
            cameraMaxY = currentArea.transform.position.y + currentArea.area.raw - cameraSizeY;

            // Place camera.
            Vector3 newPos = Vector3.zero;
            newPos.x = Mathf.Clamp(target.position.x, cameraMinX, cameraMaxX);
            newPos.y = Mathf.Clamp(target.position.y, cameraMinY, cameraMaxY);
            newPos.z = target.position.z + m_OffsetZ;
            transform.position = newPos;

            // Lock an axis if area is too small (verticaly or/and horizontaly).
            if (cameraSizeX * 2 > currentArea.area.column)
                lockHorizontal = true;
            else
                lockHorizontal = false;

            if (cameraSizeY * 2 > currentArea.area.raw)
                lockVertical = true;
            else
                lockVertical = false;
        }

        // Center the camera on the player gameobject but restrict his position in the boundaries of the current area.
        private void FollowPlayer()
        {
            Vector3 newPos = Vector3.zero;

            // Limit the camera.
            if (!lockHorizontal)
                newPos.x = Mathf.Clamp(target.position.x, cameraMinX, cameraMaxX);
            else
                newPos.x = transform.position.x;

            if (!lockVertical)
                newPos.y = Mathf.Clamp(target.position.y, cameraMinY, cameraMaxY);
            else
                newPos.y = transform.position.y;

            newPos.z = target.position.z + m_OffsetZ;

            transform.position = newPos;
        }

        #endregion

        // Original code. Smooth follow, align view with what the player look ahead.
        private void OriginalCameraScript()
        {
            /*
            // Only update lookahead pos if accelerating or changed direction
            float xMoveDelta = (target.position - m_LastTargetPosition).x;
            
            bool updateLookAheadTarget = Mathf.Abs(xMoveDelta) > lookAheadMoveThreshold; // Always true with our parameter.

            if (updateLookAheadTarget)
            {
                m_LookAheadPos = lookAheadFactor * Vector3.right * Mathf.Sign(xMoveDelta); // always 0 with our parameter.
            }

            else
            {
                m_LookAheadPos = Vector3.MoveTowards(m_LookAheadPos, Vector3.zero, Time.deltaTime * lookAheadReturnSpeed); // Never pass here.
            }

            Vector3 aheadTargetPos = target.position + m_LookAheadPos + Vector3.forward * m_OffsetZ;
            Vector3 newPos = Vector3.SmoothDamp(transform.position, aheadTargetPos, ref m_CurrentVelocity, damping);

            transform.position = newPos;
            
            m_LastTargetPosition = target.position;
            */
        }
    }
}
