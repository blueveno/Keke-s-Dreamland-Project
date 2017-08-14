using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KekeDreamLand
{
    public class TransitionManager : MonoBehaviour {

        private Animator anim;

        private void Awake()
        {
            anim = GetComponent<Animator>();
        }

        #region Transition management

        public void FadeIn()
        {
            anim.SetTrigger("FadeIn");
        }

        public void FadeOut()
        {
            anim.SetTrigger("FadeOut");
        }

        #endregion

        #region Animation events

        // Event triggered when fadeInTransition finished.
        public void OnFadeInFinished()
        {
            GameManager.instance.FadeInFinished();
        }

        #endregion
    }
}