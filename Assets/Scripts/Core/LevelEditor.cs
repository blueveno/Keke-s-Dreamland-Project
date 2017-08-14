using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Level
{
    [Range(0, 50)]
    public int raw;
    [Range(0, 50)]
    public int column;
}

namespace KekeDreamLand
{
    /// <summary>
    /// Level editor ... TODO comment / explain if necessary.
    /// </summary>
    public class LevelEditor : MonoBehaviour
    {

        [Header("Killzone and borders :")]

        public GameObject[] borderWalls;
        public GameObject killzone;

        [Header("Level configuration :")]

        public Level level;

        public bool showBorder;
        public bool showGrid;

        private Vector3 levelSize;

        private void OnDrawGizmos()
        {
            if (showBorder || showGrid)
            {
                levelSize = new Vector3(level.column, level.raw);

                killzone.transform.position = new Vector3(levelSize.x / 2, -2.5f);
                killzone.GetComponent<BoxCollider2D>().size = new Vector2(levelSize.x, 5.0f);

                for (int i = 0; i < borderWalls.Length; i++)
                {
                    borderWalls[i].GetComponent<BoxCollider2D>().size = new Vector3(2.5f, levelSize.y + 2);
                    borderWalls[i].transform.position = new Vector3(-1 + (levelSize.x + 2) * i, levelSize.y / 2 + 1);
                }

                if (showBorder)
                    DisplayBorder();

                if (showGrid)
                    DisplayGrid();
            }
        }

        private void DisplayBorder()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(levelSize / 2, levelSize);
        }

        private void DisplayGrid()
        {
            Gizmos.color = Color.white;

            Vector3 startHLine = Vector3.zero;
            Vector3 endHLine = new Vector3(levelSize.x, 0.0f);

            for (float i = 0.5f; i < level.raw; i += 0.5f)
            {
                startHLine.y = i;
                endHLine.y = i;
                Gizmos.DrawLine(startHLine, endHLine);
            }

            Vector3 startVLine = Vector3.zero;
            Vector3 endVLine = new Vector3(0.0f, levelSize.y);

            for (float i = 0.5f; i < level.column; i += 0.5f)
            {
                startVLine.x = i;
                endVLine.x = i;
                Gizmos.DrawLine(startVLine, endVLine);
            }
        }
    }

}