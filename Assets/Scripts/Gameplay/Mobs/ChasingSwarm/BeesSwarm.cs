using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KekeDreamLand {

    public class BeesSwarm : MonoBehaviour {

        public GameObject beesSwarmPrefab;
        public GameObject lastBeePrefab;

        private ForcedScrollingArea forcedScrollingArea;

        private void Awake()
        {
            forcedScrollingArea = transform.parent.parent.gameObject.GetComponent<ForcedScrollingArea>();
        }

        private void Start()
        {
            SetupSwarm();
        }

        private void SetupSwarm()
        {
            Vector2 pos = Vector2.zero;
            pos.y = - forcedScrollingArea.area.raw / 2;
            transform.localPosition = pos;

            int i = 0;
            for (i = 0; i < forcedScrollingArea.area.raw; i++)
            {
                GameObject bees = Instantiate(beesSwarmPrefab, transform);

                pos.y = i;
                bees.transform.localPosition = pos;
            }
            
            GameObject lastBee = Instantiate(lastBeePrefab, transform);
            pos.y = i;
            lastBee.transform.localPosition = pos;
        }
    }
}
