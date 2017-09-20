using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KekeDreamLand
{
    public class SawBlade : Mob
    {
        public LayerMask destroyingLayer;
        public float rayLength = 0.3f;

        private void FixedUpdate()
        {
            CheckWalls();
        }

        private void CheckWalls()
        {
            // left wall collision.
            CheckWall(Vector2.left);

            // right wall collision.
            CheckWall(Vector2.right);
        }

        private void CheckWall(Vector2 dir)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, rayLength, destroyingLayer);
            
            if (hit.collider != null && hit.collider.tag != "SpecialTiles")
            {
                Destroy(gameObject);
            }
        }
        
    }
}