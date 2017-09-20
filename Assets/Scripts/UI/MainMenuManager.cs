using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

namespace KekeDreamLand
{

    public class MainMenuManager : MonoBehaviour
    {
        [Header("Screens")]
        public GameObject titleScreen;
        public GameObject menuScreen;
        public GameObject slotScreen;
        public GameObject settingsScreen;

        [Header("SlotMenu Screen components")]
        public TextMeshProUGUI slotMenuTitle;
        public Button[] slotButtons;
        public TextMeshProUGUI[] slotTexts;
        public TextMeshProUGUI[] moreInformationTexts;

        [Header("Disclaimer components")]
        public GameObject disclaimer;
        public TextMeshProUGUI disclaimerTitle;
        public Button[] disclaimerButtons;

        private GameObject currentScreen;

        private EventSystem eventSystem;

        public int AnswerChoosen
        {
            get { return answerChoosen; }
            set { answerChoosen = value; }
        }
        private int answerChoosen = -1;

        private void Awake()
        {
            currentScreen = titleScreen;
            eventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();

            SetupSaveSlotScreen();
        }

        private void SetupSaveSlotScreen()
        {
            for (int i = 0; i < slotTexts.Length; i++)
            {
                int slot = i; // to conserve the slot index in the OnClick attribute.
                
                if (SaveLoadManager.CheckSlot(i))
                {
                    // Load data of the slot.
                    PlayerProgress progress = SaveLoadManager.LoadPlayerProgress(slot);

                    // Update slot information.
                    slotTexts[i].text = "Slot " + (slot+1).ToString() + " - " + 0 + "%"; // TODO load completion percent.
                    moreInformationTexts[i].text = "Time : 00:00"; // TODO load time played.

                    moreInformationTexts[i].gameObject.SetActive(true);
                }

                else
                {
                    moreInformationTexts[i].gameObject.SetActive(false);

                    // Update slot information.
                    slotTexts[i].text = "Slot " + (slot + 1).ToString() + " - empty";
                }
            }
        }
        
        /// <summary>
        /// Switch to the specified screen.
        /// </summary>
        /// <param name="screen">Use the public attributes</param>
        public void SwitchTo(GameObject screen)
        {
            if(currentScreen)
                currentScreen.SetActive(false);

            currentScreen = screen;
            currentScreen.SetActive(true);

            Reselect();
        }

        /// <summary>
        /// Switch to the slot screen and adapt actions depending if the player want to create or load a save.
        /// </summary>
        /// <param name="isNewGame"></param>
        public void GoToSlotScreen(bool isNewGame)
        {
            SetupActionInSlotMenu(isNewGame);

            SwitchTo(slotScreen);
        }

        // Setup action of the button.
        private void SetupActionInSlotMenu(bool isNewGame)
        {
            // TODO disclaimer when replace save or disable interaction if slot is empty (when load).

            if (isNewGame)
                slotMenuTitle.text = "Start new game :";
            else
                slotMenuTitle.text = "Load a game :";

            for (int i = 0; i < slotButtons.Length; i++)
            {
                int slot = i;

                slotButtons[i].onClick.RemoveAllListeners();

                bool slotNotEmpty = SaveLoadManager.CheckSlot(i);

                if (isNewGame) {

                    // All slot can be used to create new game.
                    slotButtons[i].interactable = true;

                    if (slotNotEmpty)
                        slotButtons[i].onClick.AddListener(() => GameManager.instance.NewGame(slot, true));
                    else
                        slotButtons[i].onClick.AddListener(() => GameManager.instance.NewGame(slot, false));
                }

                else {

                    if (slotNotEmpty)
                    {
                        slotButtons[i].interactable = true;
                        slotButtons[i].onClick.AddListener(() => GameManager.instance.LoadPlayerProgress(slot));
                    }

                    // An empty slot can't be loaded.
                    else
                        slotButtons[i].interactable = false;
                }
            }
        }

        /// <summary>
        /// Back to the main menu.
        /// </summary>
        public void Back()
        {
            if(currentScreen == menuScreen)
                SwitchTo(titleScreen);
            else
                SwitchTo(menuScreen);
        }

        /// <summary>
        /// Return true if the current screen is the titleScreen
        /// </summary>
        /// <returns></returns>
        public bool IsSpecifiedScreen(GameObject screen)
        {
            return currentScreen == screen;
        }

        public void DisplayDisclaimer(string question)
        {
            disclaimerTitle.text = question;

            disclaimer.SetActive(true);
            disclaimerButtons[0].Select();
        }

        public void ChooseDisclaimerAnswer(int answerChoosen)
        {
            this.answerChoosen = answerChoosen;
            disclaimer.SetActive(false);

            Reselect();
        }

        private void Reselect()
        {
            Transform buttons = currentScreen.transform.Find("Buttons");

            // Reselect first button if exists.
            if (buttons && buttons.childCount > 0)
                buttons.GetChild(0).gameObject.GetComponent<Button>().Select();

            // Dont work well beacause list has random order.
            /* // Select the first active and selectable gameobject.
            if (Selectable.allSelectables.Count > 0)
                eventSystem.SetSelectedGameObject(Selectable.allSelectables[0].gameObject);
            */
        }
    }

}