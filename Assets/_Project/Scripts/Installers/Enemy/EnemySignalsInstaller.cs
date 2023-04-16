using Events;
using Zenject;

public class EnemySignalsInstaller : Installer<EnemySignalsInstaller>
{
    public override void InstallBindings()
    {
        Container.DeclareSignalWithInterfaces<SignalEnemyDeath>().OptionalSubscriber();
        Container.DeclareSignalWithInterfaces<SignalEnemyTakeHit>().OptionalSubscriber();
        Container.DeclareSignal<SignalTakeDamage>().OptionalSubscriber();
        Container.DeclareSignal<SignalEnemySpawned>().OptionalSubscriber();
    }
}