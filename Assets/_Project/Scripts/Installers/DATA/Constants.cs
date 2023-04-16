using UnityEngine;

namespace Constants
{
    public static class PlayerPrefsKey
    {
        public const string LEVEL = "CurrentLevel";
    }

    #region ANIMATION

    public static class AnimationLayer
    {
        public const int DEFAULT = 0;
        public const int UPPER = 1;
        public const int CINEMACHINE = 2;
    }

    public static class AnimationClips
    {
        public static readonly int NONE = Animator.StringToHash("NONE");
        public static readonly int IDLE = Animator.StringToHash("IDLE");
        public static readonly int ATTACK = Animator.StringToHash("ATTACK");
        public static readonly int TAKEHIT = Animator.StringToHash("HIT");
        public static readonly int WALK = Animator.StringToHash("WALK");

        public static readonly int CINEMACHINE_INGAME = Animator.StringToHash("INGAME");
        public static readonly int CINEMACHINE_SHOP = Animator.StringToHash("SHOP");
    }

    public static class AnimationParams
    {
        public static readonly int WALK_SPEED = Animator.StringToHash("WalkSpeed");
        public static readonly int ATTACK_SPEED = Animator.StringToHash("AttackSpeed");
        public static readonly int ATTACK = Animator.StringToHash("AttackState");

        public static class Enemy
        {
            public static readonly int TAKEHIT = Animator.StringToHash("TakeHit");
        }
    }

    #endregion

    public static class Layer
    {
        public const int PLAYER = 1 << 6;
        public const int ENEMY = 1 << 7;
    }

    public static class TAGS

    {
        public const string Enemy = "Enemy";
        public const string Player = "Player";
        public const string Ground = "Ground";
        public const string Shop = "ShopArea";
    }

    public static class DataPaths
    {
        public const string PLAYER_STATS = "PlayerStats.json";
        public const string WORLD_STATS = "WorldStats.json";
    }
}