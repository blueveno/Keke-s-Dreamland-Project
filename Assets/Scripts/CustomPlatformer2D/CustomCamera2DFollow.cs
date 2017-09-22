using UnityEngine;

namespace KekeDreamLand
{
    public enum CameraBehaviour
    {
        // Follow Boing.
        FOLLOW,

        // Camera is forced to scroll along the area.
        FORCED_SCROLLING
    }

    public class CustomCamera2DFollow : MonoBehaviour
    {
        #region Inspector attributes

        public Transform target;

        #endregion

        #region Private attributes

        // Camera behaviour.
        private CameraBehaviour cameraBehaviour;

        // Offset with the target.
        private float m_OffsetZ;

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
                // Check if the area is a forced scrolling area.
                forcedScrollingArea = currentArea as ForcedScrollingArea;

                // Change limit and repositionate camera.
                SetupCameraLimit();
                FollowPlayer();

                // Setup camera as forced scrolling camera.
                if (forcedScrollingArea)
                {
                    forcedScrollingArea.ScrollOn = false;

                    cameraBehaviour = CameraBehaviour.FORCED_SCROLLING;
                    SetupForcedScrollingCamera();

                    forcedScrollingArea.StartCoroutine(forcedScrollingArea.StartForcedScrollingWithDelay());
                }

                // Setup as follow target camera.
                else
                {
                    cameraBehaviour = CameraBehaviour.FOLLOW;
                    forcedScrollingArea = null;
                }
            }
        }
        private AreaEditor currentArea = null;
        
        #endregion

        #region Camera follow attributes

        // Width and height covered by the camera.
        private float cameraScreenWidth;
        private float cameraScreenHeight;

        // Camera limit X and Y.
        private float cameraMinX;
        private float cameraMaxX;
        private float cameraMinY;
        private float cameraMaxY;

        // Locking if area is too small for the camera.
        private bool lockVertical;
        private bool lockHorizontal;

        #endregion

        #region Camera forced-scrolling attributes

        // Forced scrolling area.
        private ForcedScrollingArea forcedScrollingArea = null;

        // Forced scrolling destination.
        private Vector3 forcedScrollingDestination;

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

            m_OffsetZ = transform.position.z;
            
            // Recover current area.
            CurrentArea = target.transform.parent.parent.parent.GetComponent<AreaEditor>();
        }
        
        // Update camera position at the end of each frame.
        private void LateUpdate()
        {
            if (target == null)
                return;

            if (cameraBehaviour == CameraBehaviour.FOLLOW)
                FollowPlayer();
            else
                if(forcedScrollingArea.ScrollOn)
                    ForceScrolling();
        }

        #endregion

        #region Camera follow methods

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
            newPos.z = m_OffsetZ;
            transform.position = newPos;

            cameraScreenWidth = cameraSizeX * 2;
            cameraScreenHeight = cameraSizeY * 2;

            // Lock an axis if area is too small (verticaly or/and horizontaly).
            if (cameraScreenWidth > currentArea.area.column)
                lockHorizontal = true;
            else
                lockHorizontal = false;

            if (cameraScreenHeight > currentArea.area.raw)
                lockVertical = true;
            else
                lockVertical = false;
        }

        // Center the camera on the player gameobject but restrict his position in the boundaries of the current area.
        private void FollowPlayer()
        {
            Vector3 limitedPos = Vector3.zero;

            // Limit the camera.
            if (!lockHorizontal)
                limitedPos.x = Mathf.Clamp(target.position.x, cameraMinX, cameraMaxX);
            else
                limitedPos.x = transform.position.x;

            if (!lockVertical)
                limitedPos.y = Mathf.Clamp(target.position.y, cameraMinY, cameraMaxY);
            else
                limitedPos.y = transform.position.y;

            limitedPos.z = m_OffsetZ;

            transform.position = limitedPos;
        }

        #endregion

        #region Camera forced scrolling methods
        
        // Setup all gameobject linked to the scrolling.
        private void SetupForcedScrollingCamera()
        {
            // Setup moving walls.
            forcedScrollingArea.SetupMovingWalls(transform.position, cameraScreenWidth, cameraScreenHeight);
            
            // Determine forced scrolling destination
            forcedScrollingDestination = transform.position + DirectionUtility.DirectionToVector(forcedScrollingArea.scrollingDirection) * forcedScrollingArea.GetDestinationDistance(cameraScreenWidth, cameraScreenHeight);
        }

        private void ForceScrolling()
        {
            float distance = Vector3.Distance(transform.position, forcedScrollingDestination);

            if(distance >= Mathf.Epsilon)
            {
                Vector3 newPos = Vector3.MoveTowards(transform.position, forcedScrollingDestination, Time.deltaTime * forcedScrollingArea.scrollingSpeed);

                // Moves the camera, the killing zone and the blocking wall.
                transform.position = newPos;
                
                forcedScrollingArea.MoveWalls(newPos);
            }

            else
                forcedScrollingArea.ScrollOn = false;
        }

        #endregion
    }
}
