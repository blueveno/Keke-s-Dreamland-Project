using System.Collections.Generic;
using UnityEngine;

namespace KekeDreamLand
{

    public class FallingLeaves : MonoBehaviour
    {
        public GameObject leafPrefab;

        [Range(0.5f, 10.0f)]
        public float spawnTime = 2.0f;

        [Range(0.5f, 4.0f)]
        public float fallingSpeed = 1.5f;

        public Vector3 destination;

        private List<GameObject> leaves = new List<GameObject>();

        // Use this for initialization
        void Start()
        {
            InvokeRepeating("SpawnLeaf", 0.0f, spawnTime);
        }

        // Update is called once per frame
        void Update()
        {
            for (int i = 0; i < leaves.Count; i++)
            {
                float distance = Vector3.Distance(leaves[i].transform.position, transform.position + destination);

                if (distance > Mathf.Epsilon)
                    leaves[i].transform.position = Vector3.MoveTowards(leaves[i].transform.position, transform.position + destination, fallingSpeed * Time.deltaTime);
                else
                {
                    // TODO pool to recycle gameobject.
                    Destroy(leaves[i]);
                    leaves.Remove(leaves[i]);
                }
               
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(transform.position + destination, 0.1f);
        }

        void SpawnLeaf()
        {
            leaves.Add(Instantiate(leafPrefab, transform.position, Quaternion.identity, transform));
        }
    }

}
