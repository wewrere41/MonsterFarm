using Events;
using Zenject;

namespace Installers
{
    public class GameSignalsInstaller : Installer<GameSignalsInstaller>
    {
        public override void InstallBindings()
        {
            SignalBusInstaller.Install(Container);
            InstallGameSignal();
            InstallPlayerSignals();
            InstallUtiliesSignal();
        }


        private void InstallGameSignal()
        {
            Container.DeclareSignal<SignalSetActiveItemShop>().OptionalSubscriber();
            Container.DeclareSignal<SignalJoystickSetActive>().OptionalSubscriber();
            Container.DeclareSignalWithInterfaces<SignalLevelCompleted>().OptionalSubscriber();
            Container.DeclareSignal<SignalSetActiveUi>().OptionalSubscriber();
            Container.DeclareSignalWithInterfaces<SignalBaseButton>().OptionalSubscriber();
        }

        private void InstallPlayerSignals()
        {
            Container.DeclareSignalWithInterfaces<SignalRevivePlayer>().OptionalSubscriber();
            Container.DeclareSignalWithInterfaces<SignalPlayerAttack>().OptionalSubscriber();
            Container.DeclareSignal<SignalTakeDamage>().OptionalSubscriber();
            Container.DeclareSignalWithInterfaces<SignalPlayerDeath>().OptionalSubscriber();
            Container.DeclareSignal<SignalPlayerSkillReady>().OptionalSubscriber();
            Container.DeclareSignal<SignalSetActiveCollider>().OptionalSubscriber();
            Container.DeclareSignal<SignalPlayerPlaySkill>().OptionalSubscriber();
            Container.DeclareSignal<SignalPlayerSkillCompleted>().OptionalSubscriber();
            Container.DeclareSignalWithInterfaces<SignalStopSkill>().OptionalSubscriber();
            Container.DeclareSignalWithInterfaces<SignalEnemyKilled>().OptionalSubscriber();
            Container.DeclareSignalWithInterfaces<SignalSkullCountExchange>().OptionalSubscriber();
            Container.DeclareSignal<SignalInstantiateGold>().OptionalSubscriber();
            Container.DeclareSignal<SignalGoldExchange>().OptionalSubscriber();
            Container.DeclareSignalWithInterfaces<SignalPlayerLevelUp>().OptionalSubscriber();
            Container.DeclareSignalWithInterfaces<SignalPlayerItemUpgrade>().OptionalSubscriber();
            Container.DeclareSignalWithInterfaces<SignalPlayerSkillUpgrade>().OptionalSubscriber();
            Container.DeclareSignalWithInterfaces<SignalPlayBaseParticle>().OptionalSubscriber();
            Container.DeclareSignalWithInterfaces<SignalPlaySkillParticle>().OptionalSubscriber();
        }


        private void InstallUtiliesSignal()
        {
            Container.DeclareSignalWithInterfaces<SignalPlayHaptic>().OptionalSubscriber();
        }
    }
}