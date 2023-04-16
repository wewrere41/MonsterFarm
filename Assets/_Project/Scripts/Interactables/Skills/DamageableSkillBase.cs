using System.Collections;
using Events;
using UnityEngine;
using Utilities.Extensions;

public class DamageableSkillBase : MonoBehaviour
{
    private SkillParticleSignalData _skillParticleData;
    private ParticleSystem _particleSystem;
    private Collider _collider;
    private Transform _parent;
    private bool _isAlreadyHit;

    private HitType _hitType = HitType.NONE;
    private readonly Collider[] _hitObjects = new Collider[25];
    private readonly WaitForSeconds _waitForSeconds = new(0.1f);

    public enum HitType
    {
        NONE,
        TRIGGER,
        OVERLAP
    }

    private void Awake()
    {
        _particleSystem = GetComponentInChildren<ParticleSystem>();
        _collider = GetComponent<Collider>();
        _parent = transform.parent;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_skillParticleData.Damage < 0 || _isAlreadyHit)
            return;

        if (_hitType == HitType.TRIGGER)
        {
            if (!other.CompareTag(Constants.TAGS.Enemy)) return;

            other.TryGetComponent(out CollisionBridge collisionBridge);
            collisionBridge.TakeDamage(_collider, _skillParticleData.Damage);
        }

        if (_hitType == HitType.OVERLAP)
        {
            if (other.CompareTag(Constants.TAGS.Ground) is false)
                return;

            _isAlreadyHit = true;
            var hitCount = Physics.OverlapBoxNonAlloc(transform.position, new Vector3(2, 1, 2), _hitObjects,
                Quaternion.Euler(Vector3.zero), Constants.Layer.ENEMY);

            for (var i = 0; i < hitCount; i++)
            {
                var enemy = _hitObjects[i];
                enemy.TryGetComponent(out CollisionBridge collisionBridge);
                collisionBridge.TakeDamage(_collider, _skillParticleData.Damage);
            }
        }
    }

    public virtual void Initialize(SkillParticleSignalData particleData, HitType hitType = HitType.TRIGGER)
    {
        _isAlreadyHit = false;
        _hitType = hitType;
        _collider.enabled = true;

        _skillParticleData = particleData;

        if (_skillParticleData.DetachFromParent)
            transform.parent = null;

        var mainDuration = _skillParticleData.Duration > 0
            ? _skillParticleData.Duration
            : _particleSystem.main.duration;
        
        if (hitType == HitType.TRIGGER) StartCoroutine(DisableCollider());

        StartCoroutine(StopParticleCoroutine(mainDuration));
    }


    public void Stop()
    {
        if (_skillParticleData.DetachFromParent)
        {
            transform.parent = _parent;
            transform.ResetLocal();
        }

        gameObject.SetActive(false);
    }

    private IEnumerator DisableCollider()
    {
        yield return _waitForSeconds;
        _collider.enabled = false;
    }

    private IEnumerator StopParticleCoroutine(float time)
    {
        yield return new WaitForSeconds(time);
        Stop();
    }
}