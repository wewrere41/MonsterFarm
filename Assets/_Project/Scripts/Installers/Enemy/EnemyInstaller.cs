using System;
using EnemyBehavior;
using PlayerBehaviors;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using Utilities;
using Zenject;

public class EnemyInstaller : MonoInstaller
{
    [SerializeField] [TabGroup("LocalSettings")]
    private Settings _settings;


    [SerializeField] [TabGroup("ExtraSettings")]
    private ExtraSettings _extraSettings;


    public override void InstallBindings()
    {
        EnemySignalsInstaller.Install(Container);

        Container.Bind(typeof(BaseModel), typeof(EnemyModel)).To<EnemyModel>().AsSingle()
            .WithArguments(_settings.Animator, _settings.EnemyStatsSO, _settings.NavMeshAgent);

        InstallEnemyHandlers();
        InstallEnemySettings();
    }

    private void InstallEnemyHandlers()
    {
        Container.Bind<EnemyCreator>().AsSingle();
        Container.Bind<EnemyObservables>().AsSingle();
        Container.BindInterfacesTo<EnemyParticleHandler>().AsSingle();
        Container.BindInterfacesAndSelfTo<AnimationHandler>().AsSingle();
        Container.BindInterfacesTo<EnemyMoveHandler>().AsSingle();
        Container.BindInterfacesTo<EnemyHealthSystem>().AsSingle();
        Container.BindInterfacesTo<EnemyAttackSystem>().AsSingle();
        Container.BindInterfacesTo<EnemyRagdollController>().AsSingle();
    }

    private void InstallEnemySettings()
    {
        Container.BindInstance(_extraSettings.EnemyCreator).AsSingle();
        Container.BindInstance(_extraSettings.LocalStatHolder).AsSingle();
        Container.BindInstance(_extraSettings.HealthSystem).AsSingle();
        Container.BindInstance(_extraSettings.Ragdoll).AsSingle();
        Container.BindInstance(_extraSettings.ParticleSettings).AsSingle();
    }

    [Serializable]
    [HideLabel]
    public class Settings
    {
        public Animator Animator;
        public EnemyStatsSO EnemyStatsSO;
        public NavMeshAgent NavMeshAgent;
    }

    [Serializable, HideLabel]
    private class ExtraSettings
    {
        public EnemyCreator.LocalSettings EnemyCreator;
        public EnemyModel.LocalStatHolder LocalStatHolder;
        public EnemyHealthSystem.LocalSettings HealthSystem;
        public EnemyRagdollController.LocalSettings Ragdoll;
        public EnemyParticleHandler.Settings ParticleSettings;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, _extraSettings.LocalStatHolder.FollowRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, _extraSettings.LocalStatHolder.AttackRange);
    }
}