using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KekeDreamLand
{
    [CreateAssetMenu(fileName = "New Treasure", menuName = "KekeDreamLand/Treasure", order = 0)]
    public class Treasure : ScriptableObject
    {
        public string treasureName = "Default name";
        public Sprite sprite = null;
    }
}