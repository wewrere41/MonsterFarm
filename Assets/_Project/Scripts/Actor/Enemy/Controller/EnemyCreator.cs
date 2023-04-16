using System;
using EnemyBehavior;
using UnityEngine;

public class EnemyCreator
{
    #region INJECT

    private readonly EnemyModel _enemyModel;
    private readonly LocalSettings _localSettings;

    public EnemyCreator(EnemyModel enemyModel, LocalSettings localSettings)
    {
        _enemyModel = enemyModel;
        _localSettings = localSettings;
    }

    #endregion

    public void CreateEnemy(EnemyCreationData enemyCreationData)
    {
        _enemyModel.LocalStats.Guid = enemyCreationData.Guid;
        _enemyModel.BaseStatsSo = enemyCreationData.EnemyStat;
        _enemyModel.LocalStats.InitializeLocalData(_enemyModel.EnemyStatsSo, enemyCreationData.AttackRange);

        _enemyModel.GO.transform.localScale = Vector3.one * enemyCreationData.ScaleMultiplier;
        _localSettings.BaseWeaponParent.transform.GetChild(enemyCreationData.WeaponLevel).gameObject.SetActive(true);
        _localSettings.RagdollWeaponParent.transform.GetChild(enemyCreationData.WeaponLevel).gameObject.SetActive(true);

        _localSettings.BaseArmorParent.transform.GetChild(enemyCreationData.ArmorLevel).gameObject.SetActive(true);
        _localSettings.RagdollArmorParent.transform.GetChild(enemyCreationData.ArmorLevel).gameObject.SetActive(true);

        _localSettings.BaseHeadArmorParent.transform.GetChild(enemyCreationData.HelmetArmorLevel).gameObject
            .SetActive(true);
        _localSettings.RagdollHeadArmorParent.transform.GetChild(enemyCreationData.HelmetArmorLevel).gameObject
            .SetActive(true);
        

        _localSettings.BaseRenderer.sharedMaterial = _enemyModel.EnemyStatsSo.Material;
    }

    [Serializable]
    public class LocalSettings
    {
        public SkinnedMeshRenderer BaseRenderer;
        public SkinnedMeshRenderer RagdollRenderer;
        public Transform BaseWeaponParent;
        public Transform RagdollWeaponParent;
        public Transform BaseArmorParent;
        public Transform RagdollArmorParent;

        public Transform BaseHeadArmorParent;
        public Transform RagdollHeadArmorParent;
    }

    [Serializable]
    public struct EnemyCreationData
    {
        [HideInInspector] public double Guid;
        public EnemyStatsSO EnemyStat;
        [Range(1.5f, 3f)] public float AttackRange;
        [Range(0, 5)] public int WeaponLevel;
        [Range(0, 5)] public int ArmorLevel;
        [Range(0, 5)] public int HelmetArmorLevel;
        [Range(1, 5)] public float ScaleMultiplier;
    }
}