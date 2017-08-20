using UnityEngine;
using UnityEditor;

namespace KekeDreamLand
{

    public class KekeDreamlandTool : MonoBehaviour
    {

        public void SelectBoing()
        {
            Selection.activeGameObject = GameObject.FindGameObjectWithTag("Player");
        }

        public void DisplayGrid(bool displayed)
        {
            GameObject[] areas;
            areas = GameObject.FindGameObjectsWithTag("Area");
            
            foreach (GameObject g in areas)
            {
                AreaEditor ae = g.GetComponent<AreaEditor>();
                ae.showGrid = displayed;
            }

            SceneView.RepaintAll();
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

            SceneView.RepaintAll();
        }
    }
}