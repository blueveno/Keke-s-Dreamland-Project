using UnityEngine;

namespace KekeDreamLand
{
    /// <summary>
    /// Monobehaviour of an item. When the item is pick => trigger an action. Inherit all gameobject which need to be pick by the player.
    /// </summary>
    [RequireComponent(typeof(BoxCollider2D))]
    public abstract class Item : MonoBehaviour, IAction
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

                // Item is disable and store into the items collected.
                gameObject.SetActive(false);

                GameManager.instance.levelMgr.RegisterAction(this);
            }
        }

        /// <summary>
        /// Do an action when the object is picked.
        /// </summary>
        protected abstract void DoActionWhenPick();

        public void DeleteAction()
        {
            Destroy(gameObject);
        }

        public void CancelAction()
        {
            if(this is Feather)
            {
                GameManager.instance.levelMgr.FeatherCollected--;
            }

            // Remove special item from Boing.
            SpecialItem specialItem = this as SpecialItem;
            if (specialItem)
            {
                GameManager.instance.levelMgr.PickSpecialItem(specialItem.specialItemIndex, false);
            }

            gameObject.SetActive(true);
        }
    }
}