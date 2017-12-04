using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

namespace KekeDreamLand
{
    /// <summary>
    /// Manage the main menu.
    /// </summary>
    public class MainMenuManager : MonoBehaviour
    {
        #region Inspector attributes
        
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
        public GameObject[] treasuresParent;

        [Header("Disclaimer components")]
        public GameObject disclaimer;
        public TextMeshProUGUI disclaimerTitle;
        public Button[] disclaimerButtons;

        [Header("Settings screen components")]
        public Slider musicVolumeSlider;
        public Slider soundVolumeSlider;

        #endregion

        #region Private attributes

        private GameObject currentScreen;

        public int AnswerChoosen
        {
            get { return answerChoosen; }
            set { answerChoosen = value; }
        }
        private int answerChoosen = -1;

        #endregion

        #region Unity methods

        private void Awake()
        {
            currentScreen = titleScreen;

            SetupSaveSlotScreen();
        }

        private void Start()
        {
            SetupSettingsScreen();
        }

        #endregion

        #region Public methods

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

        /// <summary>
        /// Display the disclaimer to the user.
        /// </summary>
        /// <param name="question"></param>
        public void DisplayDisclaimer(string question)
        {
            disclaimerTitle.text = question;

            disclaimer.SetActive(true);

            // Preselect "yes".
            disclaimerButtons[0].Select();
        }

        /// <summary>
        /// Choose the answer of the current disclaimer.
        /// </summary>
        /// <param name="answerChoosen"></param>
        public void ChooseDisclaimerAnswer(int answerChoosen)
        {
            this.answerChoosen = answerChoosen;
            disclaimer.SetActive(false);

            Reselect();
        }

        #endregion

        #region UI Events

        public void OnClickNewGame()
        {
            GoToSlotScreen(true);
        }

        public void OnClickLoadGame()
        {
            GoToSlotScreen(false);
        }

        public void OnClickSettings()
        {
            SwitchTo(settingsScreen);

            // Select first interactable slider.
            Slider[] sliders = GetComponentsInChildren<Slider>();

            foreach (Slider s in sliders)
            {
                if (s.interactable)
                {
                    s.Select();
                    return;
                }
            }
        }

        public void OnClickQuitGame()
        {
            GameManager.instance.QuitGame();
        }

        public void OnMusicVolumeChange()
        {
            UpdateVolume(musicVolumeSlider, "MusicVolume");
        }

        public void OnSoundVolumeChange()
        {
            UpdateVolume(soundVolumeSlider, "SoundVolume");
        }

        #endregion

        #region Private methods

        private void SetupSaveSlotScreen()
        {
            for (int i = 0; i < slotTexts.Length; i++)
            {
                int slot = i; // to conserve the slot index in the OnClick attribute.

                if (SaveLoadManager.CheckSlot(i))
                {
                    // Load data of the slot.
                    PlayerProgress progress = SaveLoadManager.LoadPlayerProgress(slot);

                    // Update treasures found.
                    treasuresParent[i].SetActive(progress.treasuresFound.Count > 0);
                    // foreach treasure... display it.

                    // Update slot information.
                    slotTexts[i].text = "Slot " + (slot + 1).ToString() + " - " + 0 + "%"; // TODO load completion percent.
                    
                    moreInformationTexts[i].text = "Time : " + GetTimeFormated(progress.timePlayed);

                    moreInformationTexts[i].gameObject.SetActive(true);
                }

                else
                {
                    treasuresParent[i].SetActive(false);
                    moreInformationTexts[i].gameObject.SetActive(false);

                    // Update slot information.
                    slotTexts[i].text = "Slot " + (slot + 1).ToString() + " - empty";
                }
            }
        }

        // Setup action of the button.
        private void SetupActionInSlotMenu(bool isNewGame)
        {
            if (isNewGame)
                slotMenuTitle.text = "Start new game :";
            else
                slotMenuTitle.text = "Load a game :";

            for (int i = 0; i < slotButtons.Length; i++)
            {
                int slot = i;

                slotButtons[i].onClick.RemoveAllListeners();

                bool slotNotEmpty = SaveLoadManager.CheckSlot(i);

                if (isNewGame)
                {

                    // All slot can be used to create new game.
                    slotButtons[i].interactable = true;

                    if (slotNotEmpty)
                        slotButtons[i].onClick.AddListener(() => GameManager.instance.NewGame(slot, true));
                    else
                        slotButtons[i].onClick.AddListener(() => GameManager.instance.NewGame(slot, false));
                }

                else
                {

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

        // Reselect first interactable button if exists.
        private void Reselect()
        {
            Transform buttonsParent = currentScreen.transform.Find("Buttons");

            // Deselect current button to prevent to unselect interactable.
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);

            if (buttonsParent)
            {
                Button[] buttons = buttonsParent.GetComponentsInChildren<Button>();

                foreach (Button b in buttons)
                {
                    if (b.interactable)
                    {
                        b.Select();
                        return;
                    }
                }
            }
        }

        // Update volume sliders.
        private void SetupSettingsScreen()
        {
            if(PlayerPrefs.HasKey("MusicVolume"))
                musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume");

            if (PlayerPrefs.HasKey("SoundVolume"))
                soundVolumeSlider.value = PlayerPrefs.GetFloat("SoundVolume");
        }

        private void UpdateVolume(Slider volumeSlider, string exposedParameter)
        {
            float volume = 0.0f;

            if (volumeSlider.value == volumeSlider.minValue)
                volume = -80.0f;
            else
                volume = volumeSlider.value;

            // Update volume.
            AudioManager.instance.SetVolume(exposedParameter, volume);

            // Save player prefs.
            PlayerPrefs.SetFloat(exposedParameter, volume);
        }

        #endregion

        private string GetTimeFormated(int timePlayed)
        {
            int hours = timePlayed / 3600;
            int minutes = timePlayed / 60;

            string hoursString = hours.ToString();
            if (hoursString.Length == 1)
                hoursString = hoursString.Insert(0, "0");

            string minutesString = minutes.ToString();
            if (minutesString.Length == 1)
                minutesString = minutesString.Insert(0, "0");

            return hoursString + ":" + minutesString;
        }
    }

}