using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KekeDreamLand
{
    /// <summary>
    /// Monobehaviour of an item. When the item is pick => trigger an action.
    /// </summary>
    [RequireComponent(typeof(BoxCollider2D))]
    public abstract class Item : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag == "Player")
            {
                DoActionWhenPick();

                Destroy(gameObject.transform.parent.gameObject);
            }
        }

        protected abstract void DoActionWhenPick();
    }
}