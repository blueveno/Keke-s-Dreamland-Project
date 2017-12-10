using UnityEngine;

namespace KekeDreamLand
{
    public class SawBlade : MonoBehaviour
    {
        public LayerMask destroyingLayer;
        public float rayLength = 0.3f;

        private void FixedUpdate()
        {
            CheckWalls();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Damage player if enter in collision with.
            if (collision.gameObject.tag == "Player")
            {
                DealDamageToPlayer(collision.gameObject);
            }
        }

        private void DealDamageToPlayer(GameObject player)
        {
            player.GetComponent<BoingManager>().LifePoints -= 1;
        }

        // check side walls to destroy this saw blade when it hits it.
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
            
            if (hit.collider != null && hit.collider.tag != "Interactable")
            {
                Destroy(gameObject);
            }
        }
        
    }
}