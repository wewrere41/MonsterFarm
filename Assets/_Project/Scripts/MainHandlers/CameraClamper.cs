using System;
using PlayerBehaviors;
using UnityEngine;
using Zenject;

namespace Utilities
{
    [ExecuteInEditMode]
    public class CameraClamper : MonoBehaviour
    {
        [SerializeField] private Settings _settings;
        private PlayerFacade _playerFacade;


        [Inject]
        public void Construct(PlayerFacade playerFacade)
        {
            _playerFacade = playerFacade;
        }


        private void LateUpdate()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
                _playerFacade ??= FindObjectOfType<PlayerFacade>();
#endif
            var cameraPos = _playerFacade.transform.position + _settings.PositionOffset;
            cameraPos.x = Mathf.Clamp(cameraPos.x, _settings.XClamp.x, _settings.XClamp.y);
            transform.position = Vector3.Lerp(transform.position, cameraPos, Time.deltaTime * _settings.Damp);
        }


        [Serializable]
        public class Settings
        {
            public Vector3 PositionOffset;
            public Vector2 XClamp;
            public Vector3 RotationOffset;
            public float Damp;
        }
    }
}