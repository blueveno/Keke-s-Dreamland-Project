using UnityEngine;

namespace KekeDreamLand
{
    [System.Serializable]
    public struct Area
    {
        [Range(1, 100)]
        public int raw;
        [Range(1, 100)]
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

        [Header("Area configuration :")]

        public Area area;

        [Header("area landmark")]

        public bool showBorder;
        public bool showGrid;

        #endregion

        #region Private attributes

        private Vector3 areaSize; // Size of the area.

        #endregion

        #region Unity methods

        // Executed only on editor.
        private void OnDrawGizmos()
        {
            if (showBorder || showGrid)
            {
                areaSize = new Vector3(area.column, area.raw);

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
            killzone.transform.position = new Vector3(areaSize.x / 2, -1.25f) + transform.position;
            killzone.GetComponent<BoxCollider2D>().size = new Vector2(areaSize.x, 2.0f);

            for (int i = 0; i < borderWalls.Length; i++)
            {
                borderWalls[i].GetComponent<BoxCollider2D>().size = new Vector3(1.5f, areaSize.y + 2);
                borderWalls[i].transform.position = new Vector3(-0.5f + (areaSize.x + 1) * i, areaSize.y / 2 + 1) + transform.position;
            }
        }

        // Display the borders of this area.
        private void DisplayBorder()
        {
            Gizmos.color = Color.red;

            Gizmos.DrawWireCube(transform.position + (areaSize / 2), areaSize);
        }

        // Display the grid to help the level designer to place tiles in this area.
        private void DisplayGrid()
        {
            Gizmos.color = Color.white;

            Vector3 startHLine = transform.position;
            Vector3 endHLine = new Vector3(areaSize.x, 0.0f) + transform.position;

            float i = 0.5f;
            while(i < area.raw)
            {
                startHLine.y += 0.5f;
                endHLine.y += 0.5f;
                Gizmos.DrawLine(startHLine, endHLine);

                i += 0.5f;
            }

            Vector3 startVLine = transform.position;
            Vector3 endVLine = new Vector3(0.0f, areaSize.y) + transform.position;

            i = 0.5f;
            while (i < area.column)
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