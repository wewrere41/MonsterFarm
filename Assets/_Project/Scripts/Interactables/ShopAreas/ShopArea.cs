using System.Collections;
using DG.Tweening;
using Events;
using PlayerBehaviors;
using PlayerBehaviors.Helper;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Utilities.Extensions;
using Zenject;

public class ShopArea : MonoBehaviour
{
    #region INJECT

    private SignalBus _signalBus;
    private WorldStatsSO _worldStatsSo;
    private PlayerFacade _playerFacade;

    [Inject]
    private void Construct(SignalBus signalBus, WorldStatsSO worldStatsSo, PlayerFacade playerFacade)
    {
        _signalBus = signalBus;
        _worldStatsSo = worldStatsSo;
        _playerFacade = playerFacade;
    }

    #endregion

    [SerializeField] private TextMeshPro _goldRequirementText;
    [SerializeField] private Transform _targetTransform;

    [SerializeField] private Image _fillImage;
    [SerializeField] private GameObject[] _areaObjects;


    private readonly WaitForSeconds _waitForSeconds = new(0.02f);
    private IEnumerator _buildShopAreaCoroutine;

    private bool _triggerEnter;
    private bool _isShopBuilded;
    private float _stayTime;

    private void Start()
    {
        _goldRequirementText.text = _worldStatsSo.ShopGoldRequirement.ToString();
        _buildShopAreaCoroutine = BuildShopAreaIEnumerable();
        _isShopBuilded = _worldStatsSo.ShopEnabled;
        if (_isShopBuilded)
            BuildShopArea(true);
        else
            BuildShopWhenAnotherShopBuilded();

        void BuildShopWhenAnotherShopBuilded()
        {
            this.ObserveEveryValueChanged(x => _worldStatsSo.ShopEnabled)
                .Where(x => x)
                .Subscribe(x =>
                {
                    _isShopBuilded = true;
                    BuildShopArea(true);
                }).AddTo(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        StartCoroutine(_buildShopAreaCoroutine);
        _triggerEnter = true;
    }

    private void OnTriggerExit(Collider other)
    {
        StopCoroutine(_buildShopAreaCoroutine);
        _triggerEnter = false;
    }

    private void Update()
    {
        if (_isShopBuilded && _triggerEnter && _stayTime < 1)
        {
            _stayTime += Time.deltaTime;
            _fillImage.fillAmount = _stayTime;
            if (_stayTime >= 1)
            {
                _stayTime = 1;
                OpenShop();
            }
        }

        if (_triggerEnter is false && _stayTime > 0)
        {
            _stayTime -= Time.deltaTime;
            _fillImage.fillAmount = _stayTime;
            if (_stayTime <= 0) _stayTime = 0;
        }
    }

    private void OpenShop()
    {
        _signalBus.Fire(new SignalSetActiveItemShop(true));
        _playerFacade.transform.ChangePosition(x: _targetTransform.position.x, z: _targetTransform.position.z);
        _playerFacade.transform.rotation = Quaternion.Euler(0, 90, 0);
    }

    private IEnumerator BuildShopAreaIEnumerable()
    {
        while (_isShopBuilded is false)
        {
            if (PlayerGoldHelper.TryDecreaseGold(1))
            {
                _worldStatsSo.ShopGoldRequirement--;
                _goldRequirementText.text = _worldStatsSo.ShopGoldRequirement.ToString();
                if (_worldStatsSo.ShopGoldRequirement == 0)
                {
                    _isShopBuilded = true;
                    _worldStatsSo.ShopEnabled = true;
                    BuildShopArea(false);
                }
            }

            yield return _waitForSeconds;
        }
    }

    private void BuildShopArea(bool buildFast)
    {
        _goldRequirementText.transform.parent.gameObject.SetActive(false);
        foreach (var areaObject in _areaObjects)
        {
            if (buildFast is false)
            {
                var currentScale = areaObject.transform.localScale;
                areaObject.transform.localScale = Vector3.zero;
                areaObject.transform.DOScale(currentScale, 0.5f).SetEase(Ease.Linear);
            }

            areaObject.SetActive(true);
        }
    }
}