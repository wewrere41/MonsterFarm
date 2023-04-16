using Events;
using MainHandlers;
using PlayerBehaviors;
using PlayerBehaviors.Helper;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class HeadHunterArea : MonoBehaviour
{
    #region INJECT

    private SignalBus _signalBus;
    private PlayerFacade _playerFacade;

    [Inject]
    private void Construct(SignalBus signalBus, PlayerFacade playerFacade, GameStateManager gameStateManager)
    {
        _signalBus = signalBus;
        _playerFacade = playerFacade;
    }

    #endregion

    [SerializeField] private Image _fillImage;
    [SerializeField] Transform _goldTarget;

    private bool _triggerEnter;
    private float _stayTime;


    private void Update()
    {
        if (_triggerEnter && _stayTime < 1)
        {
            _stayTime += Time.deltaTime;
            _fillImage.fillAmount = _stayTime;
            if (_stayTime >= 1)
            {
                _stayTime = 1;
                SellSkulls();
            }
        }

        if (_triggerEnter is false && _stayTime > 0)
        {
            _stayTime -= Time.deltaTime;
            _fillImage.fillAmount = _stayTime;
            if (_stayTime <= 0) _stayTime = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        _triggerEnter = true;
    }

    private void OnTriggerExit(Collider other)
    {
        _triggerEnter = false;
    }


    private void SellSkulls()
    {
        var goldCount = _playerFacade.PlayerStatsSo.SkullCount / 10;
        if (PlayerSkullHelper.TryDecreaseSkull(goldCount * 10))
            _signalBus.Fire(new SignalInstantiateGold(goldCount, _goldTarget.position));
    }
}