using UnityEngine;

namespace KekeDreamLand
{
    /// <summary>
    /// Tool for Kekedream land dev and level designer.
    /// </summary>
    public class KekeDreamlandTool : MonoBehaviour
    {
        public void DisplayGrid(bool displayed)
        {
            GameObject[] areas;
            areas = GameObject.FindGameObjectsWithTag("Area");
            
            foreach (GameObject g in areas)
            {
                AreaEditor ae = g.GetComponent<AreaEditor>();
                ae.showGrid = displayed;
            }
        }

        public void DisplayBorder(bool displayed)
        {
            GameObject[] areas;
            areas = GameObject.FindGameObjectsWithTag("Area");

            foreach (GameObject g in areas)
            {
                AreaEditor ae = g.GetComponent<AreaEditor>();
                ae.showBorder = displayed;
            }
        }
    }
}