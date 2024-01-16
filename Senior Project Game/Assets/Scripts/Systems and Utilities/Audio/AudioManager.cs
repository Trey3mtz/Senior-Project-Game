using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyrcadian
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance;
        [SerializeField] private AudioSource _musicSource, _fxSource;

        private void Awake()
        {
            if(!Instance)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void PlaySoundFX(AudioClip clip)
        {
            _fxSource.PlayOneShot(clip);
        }

        public void PlaySoundFX(AudioClip clip, float volume)
        {
            _fxSource.PlayOneShot(clip, volume);
        }

        public void PlayMusic(AudioClip clip)
        {
            _musicSource.PlayOneShot(clip);
        }

        public void PlayRepeatSFX(AudioClip clip)
        {
            _fxSource.Play();
        }

        public void ChangeMasterVolume(float value)
        {
            AudioListener.volume = value;
        }

    }
}
