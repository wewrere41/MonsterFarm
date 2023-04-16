using System.Linq;

namespace Utilities
{
    using System;
    using System.Collections.Generic;
    using Events;
    using UnityEngine;
    using Zenject;

    public class EnemyParticleHandler : IInitializable
    {
        #region INJECT

        private readonly Settings _settings;
        private readonly SignalBus _signalBus;

        public EnemyParticleHandler(Settings settings, SignalBus signalBus)
        {
            _settings = settings;
            _signalBus = signalBus;
        }

        #endregion

        public enum ParticleType
        {
            TAKEHIT,
            DEATH,
        }


        public void Initialize()
        {
            _signalBus.Subscribe<ISignalPlayEnemyParticle>(x => PlayParticle(x.EnemyParticleSignalData.ParticleType, x.EnemyParticleSignalData.Position));
        }

        private void PlayParticle(ParticleType type, Vector3? position)
        {
            var particleData = _settings.ParticleSystems.Single(x => x.ParticleType == type);
            particleData.ParticleSystem.transform.position =
                position ?? particleData.ParticleSystem.transform.position;
            particleData.ParticleSystem.Play();
        }

        [Serializable]
        public class Settings
        {
            public ParticleData[] ParticleSystems;
        }

        [Serializable]
        public class ParticleData
        {
            public ParticleType ParticleType;
            public ParticleSystem ParticleSystem;
        }
    }
}