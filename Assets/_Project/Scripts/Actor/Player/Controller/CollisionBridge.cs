using Events;
using UnityEngine;
using Zenject;

public class CollisionBridge : MonoBehaviour
{
    [Inject] private SignalBus _signalBus;

    public void TakeDamage(Collider collider,float damage)
    {
        _signalBus.Fire(new SignalTakeDamage(collider,damage));
    }
}
