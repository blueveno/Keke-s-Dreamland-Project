using UnityEngine;

namespace KekeDreamLand
{
    /// <summary>
    /// Monobehaviour of an item. When the item is pick => trigger an action. Inherit all gameobject which need to be pick by the player.
    /// </summary>
    [RequireComponent(typeof(BoxCollider2D))]
    public abstract class Item : MonoBehaviour
    {
        /// <summary>
        /// Handle collision with the item.
        /// </summary>
        /// <param name="collision">Collider which has hit the item.</param>
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag == "Player")
            {
                DoActionWhenPick();

                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Do an action when the object is picked.
        /// </summary>
        protected abstract void DoActionWhenPick();
    }
}