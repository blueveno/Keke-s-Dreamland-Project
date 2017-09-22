using System.Collections;
using UnityEngine;

namespace KekeDreamLand
{
    /// <summary>
    /// AreaEditor. Permit to place an area anywhere, resize it and display landmark. Place also automatically the side walls and the killzone of this area.
    /// </summary>
    public class ForcedScrollingArea : AreaEditor
    {
        #region Inspector attributes
        
        [Header("Forced Scrolling Area")]
        public bool forceScrolling = false;
        public GameObject forcedScrollingKillZone;
        public GameObject forcedScrollingBlockingWall;
        public float delayBeforeScrolling;
        public float scrollingSpeed = 1.0f;
        public Direction scrollingDirection;

        #endregion

        #region Private attributes

        private Vector2 forcedScrollingOffset;
        private bool scrollOn;
        public bool ScrollOn
        {
            get { return scrollOn; }
            set { scrollOn = value; }
        }

        #endregion

        #region Unity methods

        #endregion

        #region Public methods

        /// <summary>
        /// Setup positions and sizes of the kill wall and the blocking wall.
        /// </summary>
        /// <param name="cameraPos"></param>
        /// <param name="offset"></param>
        /// <param name="size"></param>
        public void SetupMovingWalls(Vector2 cameraPos, float cameraWidth, float cameraHeight)
        {
            // Moving killzone/Wall offset and size.
            float offsetX = 0.0f;
            float offsetY = 0.0f;
            float sizeX = 1.0f;
            float sizeY = 1.0f;

            // Check destination distance and modify offset of the moving killzone depending the scrolling direction.
            if (scrollingDirection == Direction.UP || scrollingDirection == Direction.DOWN)
            {
                offsetY = -cameraHeight / 2 - 0.5f;
                if (scrollingDirection == Direction.DOWN)
                    offsetY *= -1;

                sizeX = cameraWidth;
            }

            else
            {
                offsetX = -cameraWidth / 2 - 0.5f;
                if (scrollingDirection == Direction.LEFT)
                    offsetX *= -1;

                sizeY = cameraHeight;
            }

            Vector2 wallSize = new Vector2(sizeX, sizeY);

            forcedScrollingOffset = new Vector2(offsetX, offsetY);

            forcedScrollingKillZone.transform.position = (forcedScrollingOffset * 0.92f) + cameraPos;
            forcedScrollingKillZone.GetComponent<BoxCollider2D>().size = wallSize;
            
            forcedScrollingBlockingWall.transform.position = -(forcedScrollingOffset * 1.05f) + cameraPos;
            forcedScrollingBlockingWall.GetComponent<BoxCollider2D>().size = wallSize;
        }

        /// <summary>
        /// Return the distance between current position of the camera and the finale destination of scrolling.
        /// </summary>
        /// <param name="cameraWidth"></param>
        /// <param name="cameraHeight"></param>
        /// <returns></returns>
        public float GetDestinationDistance(float cameraWidth, float cameraHeight)
        {
            float destinationDistance = 0.0f;

            if (scrollingDirection == Direction.UP || scrollingDirection == Direction.DOWN)
                destinationDistance = area.raw - cameraHeight;
            else
                destinationDistance = area.column - cameraWidth;

            return destinationDistance;
        }

        public void MoveWalls(Vector2 newPos)
        {
            forcedScrollingKillZone.transform.position = newPos + (forcedScrollingOffset * 0.92f);
            forcedScrollingBlockingWall.transform.position = newPos - (forcedScrollingOffset * 1.05f);
        }

        // Start forced scrolling after a certain delay.
        public IEnumerator StartForcedScrollingWithDelay()
        {
            yield return new WaitForSeconds(delayBeforeScrolling);
            scrollOn = true;
        }

        #endregion
    }

}