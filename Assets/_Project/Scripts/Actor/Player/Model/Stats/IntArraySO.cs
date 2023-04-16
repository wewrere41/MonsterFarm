using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Requirements", menuName = "MonsterFarm/Data/IntArray", order = -1)]
public class IntArraySO : ScriptableObject
{
    [ListDrawerSettings(ShowIndexLabels = true)]
    public int[] Array;
    
    public int this[int index] => Array[index];
}

