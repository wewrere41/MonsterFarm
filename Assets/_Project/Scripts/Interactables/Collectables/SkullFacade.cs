using System;
using System.Collections;
using Events;
using MainHandlers;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class SkullFacade : MonoBehaviour, IPoolable<Vector3, Vector3, Vector3, int, IMemoryPool>, IDisposable
{
    [Inject] private GameObservables _gameObservables;
    [Inject] private SignalBus _signalBus;
    private IMemoryPool _pool;

    [SerializeField] private GameObject _canvas;
    private Image _skullImage;
    private Camera _camera;


    private Vector3 _defaultScale;

    private bool _canvasShouldBeActive;

    private void Awake()
    {
        _camera = Camera.main;
        _skullImage = _canvas.transform.GetChild(0).GetComponent<Image>();

        _defaultScale = _skullImage.transform.localScale;


        _gameObservables.TimeScaleObservable().Where(x => _canvasShouldBeActive && x == 0)
            .Subscribe(x => _canvas.SetActive(false)).AddTo(gameObject);
        _gameObservables.TimeScaleObservable().Where(x => _canvasShouldBeActive && x == 1)
            .Subscribe(x => _canvas.SetActive(true)).AddTo(gameObject);
    }

    private IEnumerator MoveAndScaleTo(Vector3 worldPos, Vector3 uiTargetPos, Vector3 targetScale, int skullCount)
    {
        transform.position = worldPos;
        _canvasShouldBeActive = true;
        _canvas.SetActive(true);
        _skullImage.rectTransform.position = _camera.WorldToScreenPoint(transform.position);


        StartCoroutine(ScaleTo(targetScale));
        yield return StartCoroutine(MoveTo(uiTargetPos));
        _signalBus.Fire(new SignalSkullCountExchange(skullCount));
        Dispose();
    }

    private IEnumerator ScaleTo(Vector3 targetScale)
    {
        var time = 0f;
        var startScale = _skullImage.transform.localScale;

        while (time <= 1.5)
        {
            _skullImage.transform.localScale = Vector3.Lerp(startScale, targetScale, time / 1.5f);
            time += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator MoveTo(Vector3 targetPosition)
    {
        var time = 0f;
        var startPos = _skullImage.rectTransform.position;
        var offsetHeight = -20f;

        while (time <= 1.5)
        {
            offsetHeight = Mathf.Lerp(offsetHeight, 0, time / 1.5f);
            startPos.y += offsetHeight;
            _skullImage.rectTransform.position =
                Vector3.Lerp(startPos, targetPosition, time / 1.5f);

            time += Time.deltaTime;
            yield return null;
        }
    }


    #region POOL

    public void OnDespawned()
    {
        _pool = null;
        _canvasShouldBeActive = false;
        _skullImage.transform.localScale = _defaultScale;
    }

    public void OnSpawned(Vector3 worldPos, Vector3 uiTargetPos, Vector3 uiScale, int skullCount, IMemoryPool pool)
    {
        _pool = pool;
        _canvas.SetActive(false);

        StartCoroutine(MoveAndScaleTo(worldPos, uiTargetPos, uiScale, skullCount));
    }


    public void Dispose()
    {
        _pool.Despawn(this);
    }

    public class Factory : PlaceholderFactory<Vector3, Vector3, Vector3, int, SkullFacade>
    {
    }

    #endregion
}