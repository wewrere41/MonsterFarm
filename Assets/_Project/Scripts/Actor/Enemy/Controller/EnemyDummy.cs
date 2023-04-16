using System;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

[SelectionBase]
public class EnemyDummy : MonoBehaviour
{
    [Inject] private EnemyFacade.Factory _enemyFactory;
    [Inject] WorldStatsSO _worldStatsSO;

    [Title("Creation Data", titleAlignment: TitleAlignments.Centered)] [SerializeField]
    private EnemyCreator.EnemyCreationData _enemyCreationData;

    [SerializeField] private Renderer _renderer;
    private TextMeshPro _textMesh;

    private IDisposable _disposable;
    private EnemyFacade _activeEnemy;
    private bool _hasSpawned;


    private void Awake()
    {
        if (_worldStatsSO.LevelIndex > transform.parent.GetSiblingIndex())
        {
            _hasSpawned = true;
            Destroy(gameObject);
            return;
        }

        var guid = GetGuid();
        if (_worldStatsSO.DeadEnemies.Contains(guid))
        {
            _hasSpawned = true;
            Destroy(gameObject);
            return;
        }

        _enemyCreationData.Guid = guid;


#if UNITY_EDITOR
        _textMesh.gameObject.SetActive(false);
        _renderer.material = _enemyCreationData.EnemyStat.Material;
#else
         _renderer.enabled = false;
#endif
    }


    private void OnTriggerEnter(Collider other)
    {
        if (_hasSpawned) return;
        _renderer.enabled = false;
        _hasSpawned = true;
        _activeEnemy = _enemyFactory.Create(transform.position, _enemyCreationData);


        _disposable = this.ObserveEveryValueChanged(x => _activeEnemy.CurrentState)
            .Where(x => x == EnemyState.DIED).Subscribe(_ =>
            {
                _disposable.Dispose();
                Destroy(gameObject);
            }).AddTo(gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!_hasSpawned) return;
        _disposable.Dispose();
        _activeEnemy.Dispose();
        _activeEnemy = null;
        _hasSpawned = false;

#if UNITY_EDITOR
        _renderer.enabled = true;
#endif
    }

    #region HELPER

    private double GetGuid()
    {
        var position = transform.position;
        var pow = 1;
        var intY = (int)Math.Abs(position.z);
        while (intY / pow != 0)
        {
            pow *= 10;
        }

        var p = position.x >= 0 ? 1 : -1;
        return position.x * pow + intY * p;
    }

    #endregion

#if UNITY_EDITOR
    private void OnValidate()
    {
        ShowCreationData();
    }

    private void ShowCreationData()
    {
        _renderer = GetComponent<MeshRenderer>();
        _renderer.sharedMaterial = _enemyCreationData.EnemyStat.Material;
        _textMesh ??= GetComponentInChildren<TextMeshPro>(true);

        var startIndex = _enemyCreationData.EnemyStat.ToString().IndexOf("Level", StringComparison.Ordinal);


        _textMesh.text =
            $"{_enemyCreationData.EnemyStat.ToString()[startIndex..]}" +
            $"\n Weapon {_enemyCreationData.WeaponLevel}" +
            $"\n Armor {_enemyCreationData.ArmorLevel}" +
            $"\n Helmet {_enemyCreationData.HelmetArmorLevel}" +
            $"\n Scale {_enemyCreationData.ScaleMultiplier}";
    }
#endif
}