using System;
using Events;
using MoreMountains.NiceVibrations;
using UnityEngine;
using Zenject;

namespace Utilities
{
    public class AudioAndHapticManager : IInitializable
    {
        #region INJECT

        private readonly SignalBus _signalBus;
        //private readonly Settings _settings;

        public AudioAndHapticManager(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        #endregion

        private AudioSource _audioSource;

        public void Initialize()
        {
            //GetAudioSource();
            SubscribeSignals();
        }

        private void GetAudioSource()
        {
            _audioSource = Camera.main.GetComponent<AudioSource>();
        }

        private void SubscribeSignals()
        {
            _signalBus.Subscribe<ISignalPlayHaptic>(x => PlayHaptic(x.HapticType));
            // _signalBus.Subscribe<ISignalPlaySound>(x => PlaySound(x.AudioType));
        }

        #region SIGNAL VOIDS

        /*
        private void PlaySound(AudioType audioType)
        {
            _audioSource.clip = _settings.AudioClips[(int)audioType - 1];
            _audioSource.Play();
        }
        */

        private void PlayHaptic(HapticTypes hapticType) => MMVibrationManager.Haptic(hapticType);

        #endregion

        #region ENUMS

        public enum AudioType
        {
            NONE = 0,
            COLLECT = 1,
            REMOVE = 2,
        }

        #endregion

        [Serializable]
        public class Settings
        {
            public AudioClip[] AudioClips;
        }
    }
}