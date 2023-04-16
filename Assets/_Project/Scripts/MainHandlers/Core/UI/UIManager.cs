using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Events;
using Sirenix.OdinInspector;
using UniRx;
using UnityEngine;
using Zenject;

namespace MainHandlers.UI
{
    public class UIManager : IInitializable, IDisposable
    {
        #region INJECT

        private readonly BaseSettings _baseSettings;
        private readonly GameObservables _gameObservables;
        private readonly SignalBus _signalBus;
        private readonly CompositeDisposable _disposable = new CompositeDisposable();


        private UIManager(BaseSettings baseSettings,
            GameObservables gameObservables, SignalBus signalBus)
        {
            _baseSettings = baseSettings;
            _gameObservables = gameObservables;
            _signalBus = signalBus;
        }

        #endregion

        private GameObject _activeUI;
        private readonly CancellationTokenSource _cancellationTokenSource = new();

        public void Initialize()
        {
            ChangeStateWithGameState();
            _signalBus.Subscribe<SignalSetActiveUi>(x => SetActiveUI(x.IsActive));

            void ChangeStateWithGameState()
            {
                _gameObservables.GameStateObservable.Subscribe(ChangeUI).AddTo(_disposable);
            }
        }

        private void SetActiveUI(bool isActive)
        {
            _activeUI.SetActive(isActive);
        }


        private async void ChangeUI(GameStateManager.GameStates state)
        {
            _activeUI?.SetActive(false);
            _activeUI = null;
            _activeUI = await GetCurrentUI(state);
            _activeUI?.SetActive(true);
        }


        private async UniTask<GameObject> GetCurrentUI(GameStateManager.GameStates states)
        {
            return states switch
            {
                GameStateManager.GameStates.IdleState => _baseSettings.PreGameUI,
                GameStateManager.GameStates.InGameState => _baseSettings.InGameUI,
                GameStateManager.GameStates.FailState => _baseSettings.FailUI,
                _ => null
            };
        }


        public void Dispose()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            _disposable?.Dispose();
        }

        [Serializable]
        public class BaseSettings
        {
            [TabGroup("STATE UI")] public GameObject PreGameUI;
            [TabGroup("STATE UI")] public GameObject InGameUI;
            [TabGroup("STATE UI")] public GameObject FailUI;
        }
    }
}