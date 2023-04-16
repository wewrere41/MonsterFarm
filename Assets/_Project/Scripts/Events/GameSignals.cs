using MoreMountains.NiceVibrations;
using UnityEngine;
using Utilities;

namespace Events
{
    #region COMMON

    public struct SignalTakeDamage
    {
        public Collider Collider { get; }
        public float Damage { get; }

        public SignalTakeDamage(Collider collider, float damage)
        {
            Collider = collider;
            Damage = damage;
        }
    }

    #endregion

    #region UI

    public struct SignalSetActiveItemShop
    {
        public SignalSetActiveItemShop(bool isActive)
        {
            IsActive = isActive;
        }

        public bool IsActive { get; }
    }

    public struct SignalJoystickSetActive
    {
        public bool IsActive { get; }

        public SignalJoystickSetActive(bool ısActive)
        {
            IsActive = ısActive;
        }
    }

    public struct SignalSetActiveUi
    {
        public bool IsActive { get; }

        public SignalSetActiveUi(bool isActive)
        {
            IsActive = isActive;
        }
    }

    #endregion

    public struct SignalPlayHaptic : ISignalPlayHaptic
    {
        public HapticTypes HapticType { get; }

        public SignalPlayHaptic(HapticTypes hapticType)
        {
            HapticType = hapticType;
        }
    }

    public interface ISignalPlayHaptic
    {
        public HapticTypes HapticType { get; }
    }

    public interface ISignalPlaySound
    {
        public AudioAndHapticManager.AudioType AudioType { get; }
    }


    #region GAME


    public readonly struct SignalLevelCompleted : ISignalResetHealth
    {
    }

    #endregion
}