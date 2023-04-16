using Events;
using Zenject;

namespace PlayerBehaviors.Helper
{
    public class PlayerGoldHelper
    {
        #region INJECT

        private static PlayerFacade _playerFacade;
        private static SignalBus _signalBus;

        [Inject]
        public void Construct(PlayerFacade playerFacade, SignalBus signalBus)
        {
            _playerFacade = playerFacade;
            _signalBus = signalBus;
        }

        #endregion

        public static bool TryDecreaseGold(int count)
        {
            var playerGoldCount = _playerFacade.PlayerStatsSo.GoldCount;
            if (playerGoldCount - count < 0 || count == 0)
                return false;

            _signalBus.Fire(new SignalGoldExchange(-count));
            return true;
        }
    }
}