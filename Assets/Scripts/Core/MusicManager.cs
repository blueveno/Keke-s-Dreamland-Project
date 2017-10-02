using UnityEngine;
using UnityEngine.Audio;

namespace KekeDreamLand
{
    /// <summary>
    /// Music manager of the game.
    /// </summary>
    public class MusicManager : MonoBehaviour
    {
        private AudioSource music;
        private AudioMixer musicMixer;

        private void Awake()
        {
            music = GetComponent<AudioSource>();
            musicMixer = music.outputAudioMixerGroup.audioMixer;
        }

        /// <summary>
        /// Play the specified music.
        /// </summary>
        /// <param name="clip"></param>
        public void Play(AudioClip clip)
        {
            music.clip = clip;
            music.Play();
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