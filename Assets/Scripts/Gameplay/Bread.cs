using UnityEngine;
using UnityEngine.SceneManagement;

namespace KekeDreamLand
{
    /// <summary>
    /// Monobehaviour of the bread. When the bread is take. Trigger end of the level.
    /// </summary>
    public class Bread : MonoBehaviour
    {

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag == "Player")
            {
                TriggerEnd();

                Destroy(gameObject);
            }
        }

        private void TriggerEnd()
        {
            // TODO : GAMEMANAGER SWITCH TO NEXT SCENE -> COROUTINE...
            // transition fade ? other ? ...
            // FEEDBACK -> sound, particles..

            // Temporary : Reset scene.
            SceneManager.LoadScene(0);
        }
    }
}