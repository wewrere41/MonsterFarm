using UnityEngine;
using Utilities;

namespace Events
{
    public struct SignalEnemyDeath : ISignalPlayEnemyParticle
    {
        public Vector3 HitPosition { get; }
        public EnemyParticleSignalData EnemyParticleSignalData { get; }

        public SignalEnemyDeath(Vector3 hitPosition, EnemyParticleSignalData enemyParticleSignalData)
        {
            HitPosition = hitPosition;
            EnemyParticleSignalData = enemyParticleSignalData;
        }
    }


    public struct SignalEnemyTakeHit : ISignalPlayEnemyParticle
    {
        public EnemyParticleSignalData EnemyParticleSignalData { get; }

        public SignalEnemyTakeHit(EnemyParticleSignalData enemyParticleSignalData)
        {
            EnemyParticleSignalData = enemyParticleSignalData;
        }
    }

    public readonly struct SignalEnemySpawned
    {
    }

    public struct EnemyParticleSignalData
    {
        public EnemyParticleHandler.ParticleType ParticleType { get; }
        public Vector3? Position { get; }

        public EnemyParticleSignalData(EnemyParticleHandler.ParticleType particleType, Vector3? position = null)
        {
            ParticleType = particleType;
            Position = position;
        }
    }

    public interface ISignalPlayEnemyParticle
    {
        public EnemyParticleSignalData EnemyParticleSignalData { get; }
    }
}