using MoreMountains.NiceVibrations;
using UnityEngine;
using Utilities;

namespace Events
{
    #region PARTICLE

    #region BASE

    public struct SignalPlayBaseParticle : ISignalPlayBaseParticle
    {
        public SignalPlayBaseParticle(BaseParticleSignalData baseParticleSignalData)
        {
            BaseParticleSignalData = baseParticleSignalData;
        }

        public BaseParticleSignalData BaseParticleSignalData { get; }
    }

    public interface ISignalPlayBaseParticle
    {
        public BaseParticleSignalData BaseParticleSignalData { get; }
    }

    public struct BaseParticleSignalData
    {
        public PlayerParticleHandler.ParticleType ParticleType { get; }


        public BaseParticleSignalData(PlayerParticleHandler.ParticleType particleType)
        {
            ParticleType = particleType;
        }
    }

    #endregion

    #region SKILL

    public interface ISignalPlaySkillParticle
    {
        public SkillParticleSignalData SkillParticleSignalData { get; }
    }

    public struct SignalPlaySkillParticle : ISignalPlaySkillParticle
    {
        public SkillParticleSignalData SkillParticleSignalData { get; }

        public SignalPlaySkillParticle(SkillParticleSignalData skillParticleSignalData)
        {
            SkillParticleSignalData = skillParticleSignalData;
        }
    }

    public struct SkillParticleSignalData
    {
        public PlayerParticleHandler.ParticleType ParticleType { get; }
        public float Damage { get; }
        public Vector3? Position { get; }
        public float Duration { get; }
        public bool DetachFromParent { get; }

        public SkillParticleSignalData(PlayerParticleHandler.ParticleType particleType, float damage = 0,
            bool detachFromParent = false, Vector3? position = null,
            float duration = 0)
        {
            ParticleType = particleType;
            Damage = damage;
            DetachFromParent = detachFromParent;
            Duration = duration;
            Position = position;
        }
    }

    #endregion

    public interface ISignalStopParticle
    {
    }

    #endregion

    public struct SignalRevivePlayer:ISignalResetHealth,ISignalResetPosition
    {
    }

    public struct SignalBaseButton : ISignalResetHealth,ISignalResetPosition
    {
        
    }

    public interface ISignalResetPosition
    {
        
    }
    public interface ISignalResetHealth
    {
    }

    public struct SignalPlayerDeath : ISignalPlayBaseParticle
    {
        public BaseParticleSignalData BaseParticleSignalData { get; }

        public SignalPlayerDeath(BaseParticleSignalData baseParticleSignalData)
        {
            BaseParticleSignalData = baseParticleSignalData;
        }
    }

    #region ENEMY KILLED

    public struct SignalEnemyKilled : ISignalGainExperience, ISignalInstantiateSkull, ISignalSaveDeadEnemy
    {
        public int ExperienceCount { get; set; }

        public int SkullCount { get; set; }
        public Vector3 EnemyPosition { get; set; }

        public double GUID { get; }

        public SignalEnemyKilled(int experienceCount, int skullCount, Vector3 enemyPosition, double guid)
        {
            ExperienceCount = experienceCount;
            SkullCount = skullCount;
            EnemyPosition = enemyPosition;
            GUID = guid;
        }
    }


    public interface ISignalInstantiateSkull
    {
        public int SkullCount { get; }
        public Vector3 EnemyPosition { get; }
    }

    public interface ISignalGainExperience
    {
        public int ExperienceCount { get; set; }
    }

    public interface ISignalSaveDeadEnemy
    {
        public double GUID { get; }
    }

    #endregion

    #region GOLD

    public struct SignalGoldExchange
    {
        public int GoldExchangeCount { get; }


        public SignalGoldExchange(int goldExchangeCount)
        {
            GoldExchangeCount = goldExchangeCount;
        }
    }

    public struct SignalInstantiateGold
    {
        public int GoldCount { get; }
        public Vector3 GoldPosition { get; }

        public SignalInstantiateGold(int goldCount, Vector3 goldPosition)
        {
            GoldCount = goldCount;
            GoldPosition = goldPosition;
        }
    }

    #endregion

    #region SKULL

    public struct SignalSkullCountExchange
    {
        public int CollectedSkullCount { get; }

        public SignalSkullCountExchange(int collectedSkullCount)
        {
            CollectedSkullCount = collectedSkullCount;
        }
    }

    #endregion

    #region ATTACK SYSTEM

    public struct SignalPlayerAttack : ISignalPlayHaptic
    {
        public HapticTypes HapticType { get; }

        public SignalPlayerAttack(HapticTypes hapticType)
        {
            HapticType = hapticType;
        }
    }

    public struct SignalSetActiveCollider
    {
        public bool IsActive { get; }

        public SignalSetActiveCollider(bool isActive)
        {
            IsActive = isActive;
        }
    }

    #endregion

    #region SKILL

    public struct SignalPlayerSkillReady
    {
        public SkillTypes Skilltype { get; }

        public SignalPlayerSkillReady(SkillTypes skilltype)
        {
            Skilltype = skilltype;
        }
    }

    public struct SignalPlayerPlaySkill
    {
        public SkillTypes Skilltype { get; }

        public SignalPlayerPlaySkill(SkillTypes skilltype)
        {
            Skilltype = skilltype;
        }
    }

    public struct SignalPlayerSkillCompleted
    {
        public SkillTypes Skilltype { get; }

        public SignalPlayerSkillCompleted(SkillTypes skilltype)
        {
            Skilltype = skilltype;
        }
    }

    public struct SignalStopSkill : ISignalStopParticle
    {
    }

    #endregion

    #region LV

    public struct SignalPlayerLevelUp : ISignalPlayBaseParticle,ISignalResetHealth
    {
        public SignalPlayerLevelUp(BaseParticleSignalData baseParticleSignalData)
        {
            BaseParticleSignalData = baseParticleSignalData;
        }

        public BaseParticleSignalData BaseParticleSignalData { get; }
    }

    #endregion

    #region UPGRADE

    public struct SignalPlayerItemUpgrade : ISignalPlayBaseParticle

    {
        public ItemTypes ItemType { get; }

        public BaseParticleSignalData BaseParticleSignalData { get; }

        public SignalPlayerItemUpgrade(ItemTypes itemType, BaseParticleSignalData baseParticleSignalData)
        {
            ItemType = itemType;
            BaseParticleSignalData = baseParticleSignalData;
        }
    }

    public struct SignalPlayerSkillUpgrade : ISignalPlayBaseParticle
    {
        public SkillTypes SkillType { get; }

        public BaseParticleSignalData BaseParticleSignalData { get; }

        public SignalPlayerSkillUpgrade(SkillTypes skillType, BaseParticleSignalData baseParticleSignalData)
        {
            SkillType = skillType;
            BaseParticleSignalData = baseParticleSignalData;
        }
    }

    #endregion
}