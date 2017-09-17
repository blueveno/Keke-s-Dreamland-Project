using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KekeDreamLand
{
    public class LevelData : ScriptableObject
    {
        [Header("Level configuration :")]
        public string levelName = "Default name";

        public bool isSecretLevel;

        [Header("Items informations :")]
        public int totalFeathers = 0;

        [Tooltip("0:Key, 1:Raisin bread, 2:Chocolatine, 3:Sun flower seed.")]
        public bool[] itemsPresent = new bool[4];
    }
}