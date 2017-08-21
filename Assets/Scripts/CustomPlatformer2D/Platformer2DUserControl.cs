using System.Collections.Generic;

using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace KekeDreamLand
{
    public enum GamepadType
    {
        NONE, XBOX, PS, OTHER
    }

    /// <summary>
    /// Input manager. Some inputs can be observed by observer.
    /// </summary>
    [RequireComponent(typeof (PlatformerCharacter2D))]
    public class Platformer2DUserControl : MonoBehaviour
    {
        #region Private attributes

        private PlatformerCharacter2D m_Character;
        private bool m_Jump;

        // Boing script.
        private BoingManager boing;
        
        // List of Boing observers.
        private List<IObserver> observers = new List<IObserver>();

        private GamepadType gamepadUsed;

        #endregion

        #region Unity methods

        private void Awake()
        {
            m_Character = GetComponent<PlatformerCharacter2D>();
            
            boing = GetComponent<BoingManager>();
            
            CheckGamepads();
        }

        private void Update()
        {
            HandleActionsWhenBouncing();

            if (boing.IsBouncing)
                return;

            HandleActionsWhenNotBouncing();
        }

        private void FixedUpdate()
        {
            // Can't move if Boing is bouncing.
            if (boing.IsBouncing)
                return;

            HandleMoveAndCrouch();
        }

        #endregion

        #region All Actions

        // Actions which can be process during Boing is bouncing.
        private void HandleActionsWhenNotBouncing()
        {
            HandleJump();

            HandleAttack();

            HandleInteractableGameobject();
        }

        // Actions which can be process during Boing is bouncing.
        private void HandleActionsWhenBouncing()
        {
            HandleBounce();

            ToggleHUD();
        }

        // Jump when button is pressed.
        private void HandleJump()
        {
            // Can't jump if Boing is bouncing.
            if (!m_Jump)
            {
                // Read the jump input in Update so button presses aren't missed.
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");

                HandleOneSidedPlatform();

                if (m_Jump && m_Character.IsGrounded)
                    NotifyAll();
            }
        }

        // Handle input to try to pass through a one sided platform. 
        private void HandleOneSidedPlatform()
        {
            // Check gamepad and one sided platform.
            if (m_Jump && gamepadUsed != GamepadType.NONE)
            {
                // If jump has been pressed and joystick is down, pass through the one sided platform.
                if (CrossPlatformInputManager.GetAxis("Vertical") < 0)
                {
                    m_Character.MoveDown();
                    m_Jump = false;
                }
            }
            
            // If player plays with keyboard, use other control :
            if (CrossPlatformInputManager.GetButtonDown("Vertical"))
            {
                m_Character.MoveDown();
            }
        }

        private void HandleAttack()
        {
            if (CrossPlatformInputManager.GetButtonDown("Attack")  /* && !boing.IsAttacking */)
            {
                boing.Attack();
            }
        }

        private void HandleMoveAndCrouch()
        {
            // TODO Remove crouch.

            // Read the inputs.
            bool crouch = Input.GetKey(KeyCode.LeftControl);
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            // Pass all parameters to the character control script.
            m_Character.Move(h, crouch, m_Jump);
            m_Jump = false;

            // TODO : Handle input via gamepad GetAxis("Vertical"); AND via Keyboard => GetInputDown("Vertical").
        }

        // Start bouncing or stop.
        private void HandleBounce()
        {
            // Can't bounce when Boing isn't grounded.
            if (!m_Character.IsGrounded && m_Character.VSpeed != 0)
                return;

            // Start bouncing when button is pressed.
            if (CrossPlatformInputManager.GetButtonDown("Bounce") && !boing.IsBouncing && m_Character.VSpeed == 0)
            {
                boing.Bounce();

                // Stop totally move of Boing.
                m_Character.Move(0.0f, false, false);
            }

            // Stop bouncing when button is released if Boing was bouncing.
            if(CrossPlatformInputManager.GetButtonUp("Bounce") && boing.IsBouncing)
            {
                boing.StopBounce();
            }
        }

        // Handle interaction with interactable gameobject when button is pressed and object is in range.
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
        
        // Display or undisplay HUD.
        private void ToggleHUD()
        {
            if (CrossPlatformInputManager.GetButtonDown("ToggleHUD"))
            {
                GameManager.instance.ToggleHUD();
            }
        }

        // Check if a gamepad is connected. And identify it.
        private void CheckGamepads()
        {
            gamepadUsed = GamepadType.NONE;

            string[] devices = Input.GetJoystickNames();

            if (devices.Length == 0)
                return;

            foreach(string s in devices)
            {
                if(s.Contains("XBOX"))
                {
                    gamepadUsed = GamepadType.XBOX;
                }

                else
                {
                    gamepadUsed = GamepadType.OTHER;
                }
            }
        }

        #endregion

        #region Observer patern

        public void AddObserver(IObserver obs)
        {
            observers.Add(obs);
        }

        public void RemoveObserver(IObserver obs)
        {
            observers.Remove(obs);
        }

        public void NotifyAll()
        {
            foreach(IObserver obs in observers)
            {
                obs.NotifyJump();
            }
        }

        #endregion
    }
}
