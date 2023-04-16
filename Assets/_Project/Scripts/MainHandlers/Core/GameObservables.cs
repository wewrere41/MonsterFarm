using System;
using UniRx;
using UnityEngine;
using Zenject;

namespace MainHandlers
{
    public class GameObservables
    {
        #region INJECT

        private readonly TickableManager _tickableManager;
        private readonly GameStateManager _gameStateManager;

        private GameObservables(TickableManager tickableManager, GameStateManager gameStateManager)
        {
            _tickableManager = tickableManager;
            _gameStateManager = gameStateManager;
        }

        #endregion

        #region STATE

        public IObservable<GameStateManager.GameStates> GameStateObservable =>
            _gameStateManager.GameStateReactiveProperty;

        public IObservable<GameStateManager.GameStates> GameStateUpdateObservable => _tickableManager.TickStream
            .Select(x => _gameStateManager.GameStateReactiveProperty.Value);

        public IObservable<GameStateManager.GameStates> GameStateLateUpdateObservable => _tickableManager.LateTickStream
            .Select(x => _gameStateManager.GameStateReactiveProperty.Value);

        #endregion

        #region COMMON

        public IObservable<GameStateManager.GameStates> WhileInGameState() =>
            GameStateUpdateObservable.Where(x => x == GameStateManager.GameStates.InGameState);

        public IObservable<float> TimeScaleObservable() =>
            this.ObserveEveryValueChanged(x => Time.timeScale);

        #endregion

        #region PLAYER

        #region SKILL

        private readonly ReactiveProperty<bool> _canPlaySkillReactiveProperty = new(false);

        public IObservable<bool> CanPlaySkillObservable() => _canPlaySkillReactiveProperty;
        public bool SkillCanPlay => _canPlaySkillReactiveProperty.Value;

        public void SetSkillCanPlayable(bool value)
        {
            _canPlaySkillReactiveProperty.Value = value;
        }

        #endregion

        public readonly ReactiveProperty<bool> CanUnlockNextLevel = new BoolReactiveProperty(false);

        #endregion
    }
}