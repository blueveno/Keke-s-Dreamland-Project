using UnityEngine;

namespace KekeDreamLand
{
    public enum Direction
    {
        UP, DOWN, LEFT, RIGHT
    }

    public class DirectionUtility : MonoBehaviour
    {
        /// <summary>
        /// Return Vector3 associated to this direction.
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static Vector3 DirectionToVector(Direction direction)
        {
            Vector3 vDir = Vector3.zero;

            switch (direction)
            {
                case Direction.DOWN: vDir = Vector3.down; break;
                case Direction.UP: vDir = Vector3.up; break;
                case Direction.LEFT: vDir = Vector3.left; break;
                case Direction.RIGHT: vDir = Vector3.right; break;
            }

            return vDir;
        }
    }
}