using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class BaseStatsSO : ScriptableObject
{
    [SerializeField, BoxGroup("Base Stats", CenterLabel = true), JsonProperty(Order = int.MinValue)]
    public float AttackDamage;

    [SerializeField, BoxGroup("Base Stats", CenterLabel = true), JsonProperty(Order = int.MinValue)]
    public float Health;

    [SerializeField, BoxGroup("Base Stats", CenterLabel = true), Range(1, 3), JsonProperty(Order = int.MinValue)]
    public float AttackSpeed;

    [SerializeField, BoxGroup("Base Stats", CenterLabel = true), JsonProperty(Order = int.MinValue)]
    public float MovementSpeed;

    public abstract void Reset();
}