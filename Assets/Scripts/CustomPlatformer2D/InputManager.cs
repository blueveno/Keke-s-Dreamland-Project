using System.Collections.Generic;

using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace KekeDreamLand
{
    /// <summary>
    /// Type of gamepad actually used. None = Keyboard.
    /// </summary>
    public enum GamepadType
    {
        NONE, XBOX, PS, OTHER
    }

    /// <summary>
    /// Input manager. Some inputs can be observed by observer.
    /// </summary>
    public class InputManager : MonoBehaviour
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
            IdentifyGamepadIfConnected();
        }

        private void Start()
        {
            if (GameManager.instance.CurrentLevel)
            {
                m_Character = GetComponent<PlatformerCharacter2D>();
                boing = GetComponent<BoingManager>();
            }
        }

        private void Update()
        {
            // Quit with escape.
            if (CrossPlatformInputManager.GetButtonDown("Quit"))
            {
                GameManager.instance.QuitGame();
                return;
            }

            // Case of a level
            if (GameManager.instance.CurrentLevel)
                HandleLevelInteraction();

            else if (GameManager.instance.IsMainMenuScreen)
                HandleMainMenuInteraction();

            // Case of the world map.
            else if (GameManager.instance.IsWorldMapScreen)
                HandleWorldMapInteraction();

            // TODO Main screen, other case, ...
        }

        private void FixedUpdate()
        {
            if (!GameManager.instance.CurrentLevel)
                return;

            // Stop interaction with game in specific case (intern transition, end of the level, ...).
            if (GameManager.instance.CurrentLevel.IsTransition)
            {
                StopBoing();
                return;
            }

            // Can't move if Boing is bouncing.
            if (boing.IsBouncing)
                return;

            HandleMove();
        }

        #endregion

        #region World map interaction

        private bool xAxisUsed = false;
        private bool yAxisUsed = false;

        private void HandleWorldMapInteraction()
        {
            // Interact with node.
            if (Input.GetButtonDown("Jump"))
            {
                GameManager.instance.InteractWithCurrentNode();
            }

            // Move on map.
            HandleMoveOnMap();
        }

        private void HandleMoveOnMap()
        {
            float horizontal = CrossPlatformInputManager.GetAxisRaw("Horizontal");
            if (horizontal != 0)
            {

                if (!xAxisUsed)
                {
                    xAxisUsed = true;

                    if (horizontal > 0.0f)
                        GameManager.instance.MoveOnWorldMap(InputDirection.RIGHT);
                    else if (horizontal < 0.0f)
                        GameManager.instance.MoveOnWorldMap(InputDirection.LEFT);

                    return;
                }
            }
            else
                xAxisUsed = false;
            
            if (!xAxisUsed)
            {
                float vertical = CrossPlatformInputManager.GetAxisRaw("Vertical");

                if (vertical != 0)
                {
                    if (!yAxisUsed)
                    {
                        yAxisUsed = true;

                        if (vertical > 0.0f)
                            GameManager.instance.MoveOnWorldMap(InputDirection.UP);
                        else if (vertical < 0.0f)
                            GameManager.instance.MoveOnWorldMap(InputDirection.DOWN);
                    }
                }
                else
                    yAxisUsed = false;
            }
        }

        #endregion

        #region Main Menu interaction

        private void HandleMainMenuInteraction()
        {
            if (GameManager.instance.IsTitleScreen() && CrossPlatformInputManager.GetButtonDown("Submit"))
            {
                GameManager.instance.GoToMainMenu();
            }

            else if(!GameManager.instance.IsTitleScreen() && CrossPlatformInputManager.GetButtonDown("Cancel"))
            {
                GameManager.instance.BackInMenu();
            }
        }

        #endregion

        // In a level

        private void HandleLevelInteraction()
        {
            HandleLevelFinished();

            // Prevent interaction with the game in specific case (intern transition, end of the level, ...).
            if (GameManager.instance.CurrentLevel.IsTransition)
            {
                StopBoing();
                return;
            }

            HandleActionsWhenBouncing();

            if (boing.IsBouncing)
                return;

            HandleActionsWhenNotBouncing();
        }

        #region All Actions of Boing

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
                bool moveDown = CrossPlatformInputManager.GetAxis("Vertical") < 0;

                if(moveDown)
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

        private void HandleMove()
        {
            // Read the inputs.
            float h = CrossPlatformInputManager.GetAxis("Horizontal");

            // Pass all parameters to the character control script.
            m_Character.Move(h, m_Jump);
            m_Jump = false;
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

                StopBoing();
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
            if (boing.InteractableGoInRange && m_Character.IsGrounded)
            {
                if (CrossPlatformInputManager.GetAxis("Vertical") > 0.1f)
                {
                    boing.InteractableGoInRange.DoActionWhenUse();
                }
            }
        }

        // Stop instantaneously Boing at his current position.
        public void StopBoing()
        {
            m_Character.Move(0.0f, false);
        }

        #endregion

        #region Other actions

        private void HandleLevelFinished()
        {
            /*
            // Skip outro if his displaying is in progress.
            if(GameManager.instance.CurrentLevel.IsDisplayLevelOutro)
            {
                if (Input.anyKeyDown)
                {
                    GameManager.instance.CurrentLevel.SkipOutro();
                    return;
                }
            }
            */

            if (GameManager.instance.CurrentLevel.IsLevelFinished /* && !GameManager.instance.isSaving*/)
            {
                if (Input.anyKeyDown)
                {
                    GameManager.instance.LoadWorldMap();
                }
            }
        }

        // Display or undisplay HUD.
        private void ToggleHUD()
        {
            if (CrossPlatformInputManager.GetButtonDown("ToggleHUD"))
            {
                GameManager.instance.CurrentLevel.ToggleHUD();
            }
        }

        // TODO pause action

        #endregion

        #region Input manager utilities

        // Check if a gamepad is connected. If yes, identify it.
        private void IdentifyGamepadIfConnected()
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
