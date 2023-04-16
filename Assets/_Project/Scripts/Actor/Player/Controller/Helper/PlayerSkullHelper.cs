using Events;
using Zenject;

namespace PlayerBehaviors.Helper
{
    public class PlayerSkullHelper
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

        public static bool TryDecreaseSkull(int count)
        {
            var playerSkullCount = _playerFacade.PlayerStatsSo.SkullCount;
            if (playerSkullCount - count < 0 || count == 0)
                return false;

            _signalBus.Fire(new SignalSkullCountExchange(-count));
            return true;
        }
    }
}