using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KekeDreamLand
{
    /// <summary>
    /// Level configuration.
    /// </summary>
    [System.Serializable]
    public struct Level
    {
        [Range(0, 50)]
        public int raw;
        [Range(0, 50)]
        public int column;
    }

    /// <summary>
    /// AreaEditor. Permit to place an area anywhere, resize it and display landmark. Place also automatically the side walls and the killzone of this area.
    /// </summary>
    public class AreaEditor : MonoBehaviour
    {
        #region Inspector attributes
        [Header("Killzone and borders :")]

        public GameObject[] borderWalls;
        public GameObject killzone;

        [Header("Level configuration :")]

        public Level level;

        [Header("level landmark")]

        public bool showBorder;
        public bool showGrid;

        #endregion

        #region Private attributes

        private Vector3 levelSize;

        #endregion

        #region Unity methods

        // Executed only on editor.
        private void OnDrawGizmos()
        {
            if (showBorder || showGrid)
            {
                levelSize = new Vector3(level.column, level.raw);

                MoveAndScaleKillzoneAndWalls();

                if (showBorder)
                    DisplayBorder();

                if (showGrid)
                    DisplayGrid();
            }
        }

        #endregion

        #region Private methods

        // Move and scale the killzone and the side walls of this area when their dimensions or position change.
        private void MoveAndScaleKillzoneAndWalls()
        {
            killzone.transform.position = new Vector3(levelSize.x / 2, -1.25f) + transform.position;
            killzone.GetComponent<BoxCollider2D>().size = new Vector2(levelSize.x, 2.5f);

            for (int i = 0; i < borderWalls.Length; i++)
            {
                borderWalls[i].GetComponent<BoxCollider2D>().size = new Vector3(1.5f, levelSize.y + 2);
                borderWalls[i].transform.position = new Vector3(-0.5f + (levelSize.x + 1) * i, levelSize.y / 2 + 1) + transform.position;
            }
        }

        // Display the borders of this area.
        private void DisplayBorder()
        {
            Gizmos.color = Color.red;

            Gizmos.DrawWireCube(transform.position + (levelSize / 2), levelSize);
        }

        // Display the grid to help the level designer to place tiles in this area. The grid is 
        private void DisplayGrid()
        {
            Gizmos.color = Color.white;

            Vector3 startHLine = transform.position;
            Vector3 endHLine = new Vector3(levelSize.x, 0.0f) + transform.position;

            float i = 0.5f;
            while(i < level.raw)
            {
                startHLine.y += 0.5f;
                endHLine.y += 0.5f;
                Gizmos.DrawLine(startHLine, endHLine);

                i += 0.5f;
            }

            Vector3 startVLine = transform.position;
            Vector3 endVLine = new Vector3(0.0f, levelSize.y) + transform.position;

            i = 0.5f;
            while (i < level.column)
            {
                startVLine.x += 0.5f;
                endVLine.x += 0.5f;
                Gizmos.DrawLine(startVLine, endVLine);

                i += 0.5f;
            }
        }

        #endregion
    }

}