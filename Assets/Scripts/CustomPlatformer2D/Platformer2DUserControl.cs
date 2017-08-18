using System;
using System.Collections.Generic;

using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace KekeDreamLand
{
    [RequireComponent(typeof (PlatformerCharacter2D))]
    public class Platformer2DUserControl : MonoBehaviour
    {
        private PlatformerCharacter2D m_Character;
        private bool m_Jump;

        // Add by Bib'.
        private BoingManager boing;
        
        // List of Boing observers.
        private List<Observer> observers = new List<Observer>();

        private void Awake()
        {
            m_Character = GetComponent<PlatformerCharacter2D>();
            
            // Add by Bib'.
            boing = GetComponent<BoingManager>();
        }


        private void Update()
        {
            if (!m_Jump)
            {
                // Read the jump input in Update so button presses aren't missed.
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");

                if(m_Jump && m_Character.IsGrounded)
                    NotifyAll();
            }

            // Add by Bib'.
            HandleInteractableGameobject();

            ToggleHUD();
        }
        

        // Handle interaction with interactable gameobject. Add by Bib'
        private void HandleInteractableGameobject()
        {
            if (boing.InteractableGoInRange)
            {
                if (CrossPlatformInputManager.GetAxis("Vertical") > 0.1f)
                {
                    boing.InteractableGoInRange.DoActionWhenUse();
                }
            }
        }

        private void ToggleHUD()
        {
            if (CrossPlatformInputManager.GetButtonDown("ToggleHUD"))
            {
                GameManager.instance.ToggleHUD();
            }
        }


        private void FixedUpdate()
        {
            // Read the inputs.
            bool crouch = Input.GetKey(KeyCode.LeftControl);
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            // Pass all parameters to the character control script.
            m_Character.Move(h, crouch, m_Jump);
            m_Jump = false;
        }

        #region Observer patern

        public void AddObserver(Observer obs)
        {
            observers.Add(obs);
        }

        public void RemoveObserver(Observer obs)
        {
            observers.Remove(obs);
        }

        public void NotifyAll()
        {
            foreach(Observer obs in observers)
            {
                obs.NotifyJump();
            }
        }

        #endregion
    }
}
