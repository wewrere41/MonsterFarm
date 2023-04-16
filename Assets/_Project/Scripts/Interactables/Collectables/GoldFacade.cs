using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class GoldFacade : MonoBehaviour, IPoolable<Vector3, Vector3, Vector3, IMemoryPool>, IDisposable
{
    private IMemoryPool _pool;

    [SerializeField] private Image _goldImage;
    private Camera _camera;


    private Vector3 _defaultScale;

    private void Awake()
    {
        _camera = Camera.main;
        _defaultScale = _goldImage.transform.localScale;
    }

    private void MoveAndScaleTo(Vector3 worldPos, Vector3 uiTargetPos, Vector3 targetScale)
    {
        transform.position = worldPos;
        _goldImage.rectTransform.position = _camera.WorldToScreenPoint(transform.position);
        _goldImage.transform.DOScale(targetScale, 1.5f).SetEase(Ease.Linear);
        _goldImage.transform.DOMove(uiTargetPos, 1.5f).SetEase(Ease.Linear).OnComplete((Dispose));
    }

    #region POOL

    public void OnDespawned()
    {
        _pool = null;
        _goldImage.transform.localScale = _defaultScale;
    }

    public void OnSpawned(Vector3 worldPos, Vector3 uiTargetPos, Vector3 uiScale, IMemoryPool pool)
    {
        _pool = pool;
        MoveAndScaleTo(worldPos, uiTargetPos, uiScale);
    }


    public void Dispose()
    {
        _pool.Despawn(this);
    }

    public class Factory : PlaceholderFactory<Vector3, Vector3, Vector3, GoldFacade>
    {
    }

    #endregion
}