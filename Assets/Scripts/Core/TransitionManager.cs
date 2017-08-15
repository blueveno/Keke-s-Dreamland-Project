using UnityEngine;

namespace KekeDreamLand
{
    /// <summary>
    /// Manage animator of transition (FadeIn / FadeOut).
    /// </summary>
    public class TransitionManager : MonoBehaviour {

        private Animator anim;

        private void Awake()
        {
            anim = GetComponent<Animator>();
        }

        #region Transition management

        /// <summary>
        /// Trigger fadeIn animation (visible to black screen).
        /// </summary>
        public void FadeIn()
        {
            anim.SetTrigger("FadeIn");
        }

        /// <summary>
        /// Trigger fadeIn animation (black screen to visible).
        /// </summary>
        public void FadeOut()
        {
            anim.SetTrigger("FadeOut");
        }

        #endregion

        #region Animation events

        // Event triggered when a fadeIn transition animation finished.
        public void OnFadeInFinished()
        {
            GameManager.instance.FadeInFinished();
        }

        #endregion
    }
}