using System;
using System.ComponentModel;
using Events;
using PlayerBehaviors;
using SRDebugger;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class SRDebugContainer : IDisposable

{
    #region INJECT

    private WorldStatsSO _worldStatsSo;
    private PlayerFacade _playerFacade;
    private SignalBus _signalBus;


    [Inject]
    private void Construct(WorldStatsSO worldStatsSo, PlayerFacade playerFacade, SignalBus signalBus)
    {
        _worldStatsSo = worldStatsSo;
        _playerFacade = playerFacade;
        _signalBus = signalBus;
        InitializeSRDebugContainer();
    }

    #endregion

    private bool _uiIsActive=true;

    private void InitializeSRDebugContainer()
    {
        SRDebug.Instance?.AddOptionContainer(this);
    }


    #region INJECT PROPERTIES

    [Category("Game Settings"), Sort(0)]
    public void ResetCharacter()
    {
        _worldStatsSo.Reset();
        _playerFacade.PlayerStatsSo.Reset();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    [Category("Level Settings"), Sort(0)]
    public void ResetLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    [Category("Level Settings"), NumberRange(1, 2), Increment(1), Sort(1)]
    public int LevelIndex { get; set; } = 1;

    [Category("Level Settings"), Sort(2)]
    public void LoadLevel()
    {
        SceneManager.LoadScene(LevelIndex);
    }

    [Category("UI Settings"), Sort(0)]
    public void EnableDisableUI()
    {
        _signalBus.Fire(new SignalSetActiveUi(!_uiIsActive));
        _uiIsActive = !_uiIsActive;
    }

    [Category("Player Settings"), Sort(0)]
    public void GainGold()
    {
        _signalBus.Fire(new SignalInstantiateGold(99999, Vector3.zero));
    }

    #endregion


    [Category("HIDE")]
    public void Dispose()
    {
        SRDebug.Instance?.RemoveOptionContainer(this);
    }
}