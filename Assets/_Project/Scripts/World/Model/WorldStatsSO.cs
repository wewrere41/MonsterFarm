using System.Collections.Generic;
using Constants;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "WorldStats", menuName = "MonsterFarm/Stats/WorldStats")]
public class WorldStatsSO : ScriptableObject
{
    [JsonIgnore,HideInInspector]public bool GameStarted;
    public int LevelIndex;
    public bool ShopEnabled;
    [HideIf("ShopEnabled")] public int ShopGoldRequirement;
    public readonly HashSet<double> DeadEnemies = new();


    [Button]
    public void Reset()
    {
        GameStarted = false;
        LevelIndex = 0;
        ShopEnabled = false;
        ShopGoldRequirement = 10;
        DeadEnemies.Clear();
        JsonOP.RemoveData(DataPaths.WORLD_STATS);
    }
}