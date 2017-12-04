using UnityEngine;
using UnityEngine.Audio;

namespace KekeDreamLand
{
    // TODO Audio manager (manage sounds in a list (in inspector) and store it into a dictionnary in runtime).

    /// <summary>
    /// Audio manager of the game.
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        [Header("Music")]
        public AudioSource music;
        public AudioClip mainMenuMusic;
        public AudioClip worldMapMusic;

        [Header("Sounds")]
        public AudioSource[] soundSources; // Sound sources with various pitch.

        public static AudioManager instance;
        private AudioMixer musicMixer;

        private void Awake()
        {
            if (instance == null)
                instance = this;

            else
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);

            SetupAudio();

            GameManager.instance.audioMgr = this;
        }

        private void SetupAudio()
        {
            musicMixer = music.outputAudioMixerGroup.audioMixer;

            if (PlayerPrefs.HasKey("MusicVolume"))
                SetVolume("MusicVolume", PlayerPrefs.GetFloat("MusicVolume"));

            if (PlayerPrefs.HasKey("SoundVolume"))
                SetVolume("SoundVolume", PlayerPrefs.GetFloat("SoundVolume"));
        }

        /// <summary>
        /// Play the specified music.
        /// </summary>
        /// <param name="clip"></param>
        public void PlayMusic(AudioClip clip)
        {
            if(clip == null)
            {
                Debug.Log("No music to play");
            }

            music.clip = clip;
            music.Play();
        }

        public void PlaySound(string soundToPlay)
        {
            // TODO search the sound in a dictionnary then play the sound.

            // Else debug log
            Debug.Log(soundToPlay + " is missing");
        }

        /// <summary>
        /// Update the volume of the music.
        /// </summary>
        /// <param name="newVolume"></param>
        public void SetVolume(string exposedParameter, float newVolume)
        {
            musicMixer.SetFloat(exposedParameter, newVolume);
        }
    }
}