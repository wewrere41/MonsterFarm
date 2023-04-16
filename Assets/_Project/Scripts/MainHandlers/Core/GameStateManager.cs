using System;
using Events;
using UniRx;
using UnityEngine;
using Utilities;
using Zenject;

namespace MainHandlers
{
    public class GameStateManager : IInitializable, IDisposable
    {
        public enum GameStates
        {
            IdleState,
            InGameState,
            FailState,
            FinishState,
            None
        }

        #region INJECT

        private readonly SignalBus _signalBus;
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();


        private GameStateManager(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        #endregion

        public readonly ReactiveProperty<GameStates> GameStateReactiveProperty = new(GameStates.None);

        public void Initialize()
        {
            Application.targetFrameRate = 60;

            ChangeStateToIdle();
            ChangeToInGameWhenPress();

            void ChangeStateToIdle()
            {
                Observable.Timer(TimeSpan.FromSeconds(0.01))
                    .Subscribe(x => ChangeState(GameStates.IdleState))
                    .AddTo(_compositeDisposable);
            }

            void ChangeToInGameWhenPress()
            {
                Observable.EveryUpdate().Select(x => GameStateReactiveProperty.Value)
                    .Where(x => GameStateReactiveProperty.Value == GameStates.IdleState &&
                                Input.GetMouseButtonDown(0))
                    .Subscribe(x => ChangeState(GameStates.InGameState)).AddTo(_compositeDisposable);
            }
        }


        public void ChangeState(GameStates nextState) => GameStateReactiveProperty.Value = nextState;


        public void Dispose()
        {
            _compositeDisposable?.Dispose();
        }
    }
}