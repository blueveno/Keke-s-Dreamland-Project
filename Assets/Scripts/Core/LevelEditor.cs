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

                killzone.transform.position = new Vector3(levelSize.x / 2, -1.25f) + transform.position;
                killzone.GetComponent<BoxCollider2D>().size = new Vector2(levelSize.x, 2.5f);

                for (int i = 0; i < borderWalls.Length; i++)
                {
                    borderWalls[i].GetComponent<BoxCollider2D>().size = new Vector3(1.5f, levelSize.y + 2);
                    borderWalls[i].transform.position = new Vector3(-0.5f + (levelSize.x + 1) * i, levelSize.y / 2 + 1) + transform.position;
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
            Gizmos.DrawWireCube(transform.position + (levelSize / 2), levelSize);
        }

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
    }

}