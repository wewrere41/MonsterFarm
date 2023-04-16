using System;
using System.Linq;
using Events;
using UnityEngine;
using Zenject;

namespace Utilities
{
    public class PlayerParticleHandler : IInitializable
    {
        #region INJECT

        private readonly Settings _settings;
        private readonly SignalBus _signalBus;

        public PlayerParticleHandler(Settings settings, SignalBus signalBus)
        {
            _settings = settings;
            _signalBus = signalBus;
        }

        #endregion

        private ParticleSystem _lastParticle;

        public enum ParticleType
        {
            NONE,
            LEVELUP,
            TAKEHIT,
            DEATH,
            SKILL_UPGRADE,
            ITEM_UPGRADE,
            SKILL_0,
            SKILL_1,
            SKILL_2
        }

        public void Initialize()
        {
            _signalBus.Subscribe<ISignalPlayBaseParticle>(x => PlayParticle(x.BaseParticleSignalData));
            _signalBus.Subscribe<ISignalPlaySkillParticle>(x => PlayParticle(x.SkillParticleSignalData));
            _signalBus.Subscribe<ISignalStopParticle>(StopParticle);
        }

        private void PlayParticle(BaseParticleSignalData signalData)
        {
            var particle = _settings.ParticleSystems
                .Single(x => x.ParticleType == signalData.ParticleType).ParticleSystem;

            particle.Play();
        }

        private void PlayParticle(SkillParticleSignalData signalData)
        {
            _lastParticle = _settings.ParticleSystems
                .Single(x => x.ParticleType == signalData.ParticleType).ParticleSystem;

            _lastParticle.gameObject.SetActive(true);
            _lastParticle.TryGetComponent(out DamageableSkillBase skill);
            skill.Initialize(signalData);
        }

        private void StopParticle()
        {
            if (_lastParticle == null) return;

            _lastParticle.Stop();
            _lastParticle.TryGetComponent(out DamageableSkillBase skill);
            skill.Stop();
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