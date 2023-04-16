using System;

[Flags]
public enum PlayerState
{
    NONE = 0,
    WALK = 1 << 0,
    ATTACK = 1 << 1,
    ATTACKEND = 1 << 2,
    SKILL = 1 << 3,
    DIED = 1 << 4,
    ATTACKABLES = WALK | ATTACKEND
}

[Flags]
public enum EnemyState
{
    NONE = 0,
    IDLE = 1 << 0,
    CHASE = 1 << 1,
    MOVEBACK = 1 << 2,
    TAKINGDAMAGE = 1 << 3,
    CANATTACK = 1 << 4,
    ATTACK = 1 << 5,
    ATTACKEND = 1 << 6,
    DIED = 1 << 7,

    ATTACKSTATES = CANATTACK | ATTACK | ATTACKEND,
}