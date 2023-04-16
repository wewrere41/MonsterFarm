using System.Collections;
using Constants;
using Events;
using UnityEngine;

public class CornetSpawner : MonoBehaviour
{
    [SerializeField] private DamageableSkillBase _childSkillBase;
    [SerializeField] private GameObject _startPoint;
    [SerializeField] private GameObject _endPoint;
    [SerializeField] private float _rateOfFire;
    [SerializeField] private float _quantity;
    [SerializeField] private float _radius;

    private readonly Collider[] _hitColliders = new Collider[2];

    public void InitializeSpawner(SkillParticleSignalData skillParticleSignalData)
    {
        var time = _quantity * _rateOfFire + 3f;
        StartCoroutine(SpawnVFX(skillParticleSignalData, time));
    }

    private IEnumerator SpawnVFX(SkillParticleSignalData skillParticleSignalData, float time)
    {
        for (var i = 0; i < _quantity; i++)
        {
            var startPos = CreateRandomPos(_startPoint.transform.position);

            Vector3 endPos;
            int hitCount;
            Collider hit;

            do
            {
                endPos = CreateRandomPos(_endPoint.transform.position);
                hitCount = FindHitColliders(endPos);
                hit = _hitColliders[0];
            } while (hitCount > 1 || hit == null || hit.gameObject.CompareTag(TAGS.Ground) is false);


            var cornet = Instantiate(_childSkillBase, startPos, Quaternion.identity);
            cornet.transform.LookAt(endPos);

            cornet.Initialize(skillParticleSignalData, DamageableSkillBase.HitType.OVERLAP);

            yield return new WaitForSeconds(_rateOfFire);
        }
    }

    private Vector3 CreateRandomPos(Vector3 point)
    {
        return new Vector3(point.x + Random.Range(-_radius, _radius),
            point.y,
            point.z + Random.Range(-_radius, _radius));
    }

    private int FindHitColliders(Vector3 endPos)
    {
        return Physics.OverlapBoxNonAlloc(endPos, new Vector3(0.5f, 0.5f, 0.5f), _hitColliders, Quaternion.identity,
            1 << 8);
    }
}