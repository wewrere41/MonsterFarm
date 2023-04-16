using System;
using Constants;
using Events;
using MainHandlers;
using UniRx;
using Zenject;

namespace PlayerBehaviors
{
    public class PlayerColliderHandler : IInitializable, IDisposable
    {
        #region INJECT

        private readonly PlayerObservables _playerObservables;
        private readonly SignalBus _signalBus;
        private readonly GameObservables _gameObservables;
        private readonly CompositeDisposable _compositeDisposable = new();

        public PlayerColliderHandler(PlayerObservables playerObservables, SignalBus signalBus,
            GameObservables gameObservables)
        {
            _playerObservables = playerObservables;
            _signalBus = signalBus;
            _gameObservables = gameObservables;
        }

        #endregion

        public void Initialize()
        {
            _playerObservables.PlayerTriggerEnterObservable.Where(x => x.CompareTag(TAGS.Shop))
                .Subscribe(x =>
                {
                    _gameObservables.SetSkillCanPlayable(false);
                    _signalBus.AbstractFire<SignalStopSkill>();
                }).AddTo(_compositeDisposable);
        }

        public void Dispose()
        {
            _compositeDisposable?.Dispose();
        }
    }
}