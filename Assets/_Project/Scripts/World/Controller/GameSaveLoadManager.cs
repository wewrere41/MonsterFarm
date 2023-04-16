using Constants;
using Newtonsoft.Json.Utilities;
using PlayerBehaviors;
using UnityEngine;
using Zenject;

public class GameSaveLoadManager : MonoBehaviour
{
    [Inject] private PlayerFacade _playerFacade;
    [Inject] private WorldStatsSO _worldStatsSo;

    #region DESERIALIZE

    private void Awake()
    {
        if (_worldStatsSo.GameStarted is false && _playerFacade.PlayerStatsSo.LoadFromJson)
        {
            AotHelper.EnsureList<double>();
            DeserializePlayerStats();
            DeserializeWorldData();
            _worldStatsSo.GameStarted = true;
        }
    }

    private void DeserializePlayerStats() =>
        JsonOP.DeserializeSo(DataPaths.PLAYER_STATS, _playerFacade.PlayerStatsSo);

    private void DeserializeWorldData() =>
        JsonOP.DeserializeSo(DataPaths.WORLD_STATS, _worldStatsSo);

    #endregion

    #region SERIALIZE

    private void SerializePlayerStats() =>
        JsonOP.SerializeData(_playerFacade.PlayerStatsSo, DataPaths.PLAYER_STATS);

    private void SerializeWorldStats() => JsonOP.SerializeData(_worldStatsSo, DataPaths.WORLD_STATS);


    private void OnApplicationQuit()
    {
        SerializePlayerStats();
        SerializeWorldStats();
        _worldStatsSo.GameStarted = false;
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            SerializePlayerStats();
            SerializeWorldStats();
        }
    }

    #endregion
}