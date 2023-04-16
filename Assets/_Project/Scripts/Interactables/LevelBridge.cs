using Events;
using MainHandlers;
using UnityEngine;
using Zenject;

public class LevelBridge : MonoBehaviour
{
    [Inject] private GameObservables _gameObservables;
    [Inject] private SignalBus _signalBus;

    [SerializeField] private Collider _collider;
    [SerializeField] private Collider _trigger;
    private bool _isActive = true;

    private void OnCollisionEnter(Collision collision)
    {
        if (_isActive && collision.gameObject.CompareTag(Constants.TAGS.Player) &&
            _gameObservables.CanUnlockNextLevel.Value)
            _collider.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(Constants.TAGS.Player) && _gameObservables.CanUnlockNextLevel.Value)
        {
            _isActive = false;
            _trigger.enabled = false;
            _collider.enabled = true;
            _signalBus.AbstractFire<SignalLevelCompleted>();
        }
    }
}