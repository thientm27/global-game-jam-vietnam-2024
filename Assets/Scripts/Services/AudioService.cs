using System.Collections.Generic;
using UnityEngine;
using System;
using Audio;

namespace Services
{
    public class AudioService 
    {
        public event Action<bool> OnSoundChanged;
        public event Action<float> OnSoundVolumeChanged;

        public event Action<bool> OnMusicChanged;
        public event Action<float> OnMusicVolumeChanged;

        private bool soundOn;
        private bool musicOn;
        private float soundVolume;
        private float musicVolume;

        private bool vibrateOn;

        private Music music;
        private Dictionary<string, AudioSource> soundAudioSources;

        // Cache
        private Dictionary<string, float> soundVolumes = new();

        public AudioService(Music music, List<Sound> sounds, GameObject soundObject)
        {
            this.music = music;
            this.music.Initialized(this);

            soundAudioSources = new Dictionary<string, AudioSource>();
            foreach (var sound in sounds)
            {
                AudioSource soundSource = soundObject.AddComponent<AudioSource>();
                soundSource.clip = sound.AudioClip;
                soundSource.volume = sound.Volume;
                soundSource.playOnAwake = false;
                soundAudioSources.Add(sound.Name, soundSource);
            }

            foreach (var audioSource in soundAudioSources)
            {
                soundVolumes.Add(audioSource.Key, audioSource.Value.volume);
            }
        }

        // Play Music
        public void PlayMusic()
        {
            if (musicOn == true && musicVolume > 0.0f)
            {
                music.PlayMusic("music");
            }
        }

        // Fade Music
        public void FadeMusic(float time)
        {
            if (musicOn == true && musicVolume > 0.0f)
            {
                music.FadeMusic("music", time);
            }
        }

        // End
        public void StopAllSound()
        {
            foreach (var audioSource in soundAudioSources)
            {
                audioSource.Value.Stop();
            }
        }

        public void StopMusic()
        {
            music.StopMusic("music");
        }

        public bool IsMusicPlaying()
        {
            return music.IsMusicPlaying("music");
        }

        public void SetMusicVolume(float volume)
        {
            MusicVolume = volume;
        }

        public void SetSoundVolume(float volume)
        {
            SoundVolume = volume;
        }

        public void SetVibrate(bool isOn)
        {
            VibrateOn = isOn;
        }

        public void Vibrate()
        {
#if UNITY_ANDROID
			if(vibrateOn == true)
			{
				Handheld.Vibrate();
			}
#endif
        }

        #region Get - Set

        // GET - SET
        public float SoundVolume
        {
            get { return soundVolume; }
            set
            {
                soundVolume = value;
                foreach (var audioSource in soundAudioSources)
                {
                    audioSource.Value.volume = soundVolume * soundVolumes[audioSource.Key];
                }

                OnSoundVolumeChanged?.Invoke(soundVolume);
            }
        }

        public float MusicVolume
        {
            get { return musicVolume; }
            set
            {
                musicVolume = value;
                OnMusicVolumeChanged?.Invoke(musicVolume);
            }
        }
        public bool SoundOn
        {
            get { return soundOn; }
            set
            {
                soundOn = value;
                if (SoundOn == false)
                {
                    StopAllSound();
                }

                OnSoundChanged?.Invoke(soundOn);
            }
        }

        public bool MusicOn
        {
            get { return musicOn; }
            set
            {
                musicOn = value;
                if (musicOn == true && musicVolume > 0)
                {
                    music.PlayMusic("music");
                }
                else
                {
                    music.StopMusic("music");
                }

                OnMusicChanged?.Invoke(musicOn);
            }
        }
        public bool VibrateOn
        {
            get { return vibrateOn; }
            set { vibrateOn = value; }
        }

        // AUDIO PLAY

        public void PlaySound(SoundToPlay nameSound)
        {
            if (!soundOn)
            {
                return;
            }

            soundAudioSources[nameSound.ToString()].Play();
        }

        #endregion
    }

    public enum SoundToPlay
    {
        Hit,
        Forget,
        BrokeGlass,
        AddTooth,
    }
}