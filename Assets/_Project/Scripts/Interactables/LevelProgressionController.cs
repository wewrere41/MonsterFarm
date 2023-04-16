using System;
using Events;
using MainHandlers;
using PlayerBehaviors;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class LevelProgressionController : IInitializable
{
    #region INJECT

    private readonly SignalBus _signalBus;
    private readonly WorldStatsSO _worldStatsSo;
    private readonly PlayerFacade _playerFacade;
    private readonly GameObservables _gameObservables;
    private readonly Settings _settings;

    public LevelProgressionController(SignalBus signalBus, WorldStatsSO worldStatsSo, Settings settings,
        PlayerFacade playerFacade, GameObservables gameObservables)
    {
        _signalBus = signalBus;
        _worldStatsSo = worldStatsSo;
        _settings = settings;
        _playerFacade = playerFacade;
        _gameObservables = gameObservables;
    }

    #endregion

    private int _enemyCount;

    public void Initialize()
    {
        _signalBus.Subscribe<ISignalResetPosition>(ResetPosition);
        _signalBus.Subscribe<ISignalSaveDeadEnemy>(AddDeadEnemy);
        _signalBus.Subscribe<SignalLevelCompleted>(UpdateLevel);
        SetActiveLevel(true);
    }

    private void ResetPosition()
    {
        var startPos = GetStartPosition();
        _playerFacade.transform.position = startPos;
        _playerFacade.PlayerStatsSo.LastPosition = startPos;
    }

    private Vector3 GetStartPosition()
    {
        var startPos = _settings.StartPositionsParent.GetChild(_worldStatsSo.LevelIndex).position;
        startPos.y = _playerFacade.transform.position.y;
        return startPos;
    }

    private void AddDeadEnemy(ISignalSaveDeadEnemy signal)
    {
        _worldStatsSo.DeadEnemies.Add(signal.GUID);
        _enemyCount--;
        if (_enemyCount == 0)
            _gameObservables.CanUnlockNextLevel.Value = true;
    }

    private void UpdateLevel()
    {
        _worldStatsSo.LevelIndex++;
        _worldStatsSo.DeadEnemies.Clear();
        if (_worldStatsSo.LevelIndex == 5)
        {
            _worldStatsSo.LevelIndex = 0;
            var nextLevelIndex = SceneManager.GetActiveScene().buildIndex + 1;
            SceneManager.LoadScene(nextLevelIndex >= SceneManager.sceneCountInBuildSettings ? 2 : nextLevelIndex);
        }
        else
        {
            SetActiveLevel();
        }
    }

    private void SetActiveLevel(bool firstInitialize = false)
    {
        var currentLevelEnemies = _settings.EnemyParent.transform.GetChild(_worldStatsSo.LevelIndex);
        _enemyCount = currentLevelEnemies.childCount - _worldStatsSo.DeadEnemies.Count;
        _gameObservables.CanUnlockNextLevel.Value = _enemyCount == 0;
        if (firstInitialize && _worldStatsSo.DeadEnemies.Count == 0)
            _playerFacade.PlayerStatsSo.LastPosition = GetStartPosition();
    }

    [Serializable]
    public class Settings
    {
        public Transform EnemyParent;
        public Transform StartPositionsParent;
    }
}