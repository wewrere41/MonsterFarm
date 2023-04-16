using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Events;
using MoreMountains.NiceVibrations;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace PlayerBehaviors
{
    public class PlayerGoldHandler : IInitializable, IDisposable
    {
        #region INJECT

        private readonly PlayerModel _playerModel;
        private readonly UISettings _uiSettings;
        private readonly SignalBus _signalBus;
        private readonly GoldFacade.Factory _goldFactory;

        public PlayerGoldHandler(PlayerModel playerModel, UISettings uiSettings, SignalBus signalBus,
            GoldFacade.Factory goldFactory)
        {
            _playerModel = playerModel;
            _uiSettings = uiSettings;
            _signalBus = signalBus;
            _goldFactory = goldFactory;
        }

        #endregion

        private readonly CompositeDisposable _compositeDisposable = new();
        private bool _isInstantiating;

        public void Initialize()
        {
            _signalBus.Subscribe<SignalInstantiateGold>(x => InstantiateGolds(x.GoldCount, x.GoldPosition));
            _signalBus.Subscribe<SignalGoldExchange>(x => UpdateGoldCount(x.GoldExchangeCount, 0.1f, true));

            UpdateGoldText();
        }

        private async Task InstantiateGolds(int goldCount, Vector3 position)
        {
            _isInstantiating = true;
            var objCount = goldCount < 20 ? goldCount : 20;
            var startTime = Time.time;
            for (var i = 0; i < objCount; i++)
            {
                _goldFactory.Create(position, _uiSettings.GoldImage.transform.position,
                    _uiSettings.GoldImage.transform.lossyScale);
                _signalBus.Fire(new SignalPlayHaptic(HapticTypes.Success));
                await Task.Delay(TimeSpan.FromSeconds(0.05));
            }

            var endTime = Time.time;
            var delayTime = 1.5 - (endTime - startTime);

            await Task.Delay(TimeSpan.FromSeconds(delayTime));

            var textTweenDuration = objCount * 0.05f;
            UpdateGoldCount(goldCount, textTweenDuration);
            await Task.Delay(TimeSpan.FromSeconds(textTweenDuration));
            _isInstantiating = false;
        }

        private async void UpdateGoldCount(int goldCount, float time, bool waitBefore = false)
        {
            if (waitBefore && _isInstantiating) await UniTask.WaitUntil(() => _isInstantiating == false);

            UpdateGoldText(goldCount, time);
            _playerModel.PlayerStatsSo.GoldCount += goldCount;
        }

        private void UpdateGoldText(int gainedGoldCount = 0, float time = 0)
        {
            var currentGold = _playerModel.PlayerStatsSo.GoldCount;

            DOTween.To(() => currentGold, x => currentGold = x,
                    currentGold + gainedGoldCount, time).SetEase(Ease.Linear)
                .OnUpdate(() => _uiSettings.GoldText.text = currentGold.ToString());
        }

        public void Dispose()
        {
            _compositeDisposable?.Dispose();
        }

        [Serializable]
        public class UISettings
        {
            public Image GoldImage;
            public TextMeshProUGUI GoldText;
        }
    }
}