using System;
using Constants;
using Events;
using MainHandlers;
using UniRx;
using UnityEngine;
using Zenject;

namespace PlayerBehaviors
{
    public class PlayerMoveHandler : IInitializable, IDisposable
    {
        #region INJECT

        private readonly PlayerModel _model;
        private readonly GameObservables _gameObservables;
        private readonly PlayerObservables _playerObservables;
        private readonly AnimationHandler _animationHandler;

        private readonly UISettings _uıSettings;
        private readonly SignalBus _signalBus;
        private readonly CompositeDisposable _compositeDisposable = new();

        private PlayerMoveHandler(PlayerModel model, GameObservables gameObservables,
            AnimationHandler animationHandler, UISettings uıSettings, PlayerObservables playerObservables,
            SignalBus signalBus)
        {
            _model = model;
            _gameObservables = gameObservables;
            _animationHandler = animationHandler;
            _uıSettings = uıSettings;
            _playerObservables = playerObservables;
            _signalBus = signalBus;
        }

        #endregion

        #region MoveFields

        private float _directionMagnitude;

        #endregion

        public void Initialize()
        {
            LoadStartPosition();
            SetActiveJoystick(true);
            _signalBus.Subscribe<SignalJoystickSetActive>(x => SetActiveJoystick(x.IsActive));
            _signalBus.Subscribe<SignalPlayerDeath>(OnPlayerDeath);
            _signalBus.Subscribe<SignalRevivePlayer>(x => SetActiveJoystick(true));
            _signalBus.Subscribe<ISignalResetPosition>(ResetMovement);
            _playerObservables.WhileAlive().Subscribe(x => CheckInputs()).AddTo(_compositeDisposable);
        }

        private void LoadStartPosition()
        {
            var lastPosition = _model.PlayerStatsSo.LastPosition;
            if (_model.PlayerStatsSo.LoadPosition && lastPosition != Vector3.zero)
                _model.GO.transform.position = lastPosition;
        }

        private void SetActiveJoystick(bool isActive)
        {
            _uıSettings.Joystick.gameObject.SetActive(isActive);
            _uıSettings.Joystick.OnPointerUp(null);
        }

        private void OnPlayerDeath()
        {
            SetActiveJoystick(false);
            ResetMovement();
        }

        private void ResetMovement()
        {
            _model.RigidBody.velocity = Vector3.zero;
            _animationHandler.ResetAnimationState();
        }

        #region MOVEMENT

        private void CheckInputs()
        {
            if (Input.GetMouseButton(0))
            {
                CalculateRotationAndMove();
            }

            if (Input.GetMouseButton(0) is false)
            {
                if (_directionMagnitude > 0)
                {
                    _directionMagnitude -= Time.deltaTime * 1.25f;
                    _animationHandler.SetFloat(AnimationParams.WALK_SPEED, _directionMagnitude);
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                _model.RigidBody.velocity = Vector3.zero;
                _model.RigidBody.angularVelocity = Vector3.zero;
            }
        }

        private void CalculateRotationAndMove()
        {
            var direction = new Vector3(_uıSettings.Joystick.Horizontal, 0, _uıSettings.Joystick.Vertical);
            _directionMagnitude = direction.magnitude;

            Move(direction);
            RotatePlayer(direction);
        }

        private void Move(Vector3 direction)
        {
            _model.RigidBody.velocity = direction * GetMovementSpeed * _directionMagnitude;
            //if (_playerObservables.TargetEnemy != null)
            //{
            //    var dot = Vector3.Dot(direction,
            //        _playerObservables.TargetEnemy.transform.forward);
            //    _animationHandler.SetFloat(AnimationParams.WALK_SPEED,
            //        Mathf.Lerp(_animationHandler.GetFloat(AnimationParams.WALK_SPEED),
            //            _directionMagnitude * (dot > 0 ? -1 : 1),
            //            Time.deltaTime * 4));
            //}
            //else
            _animationHandler.SetFloat(AnimationParams.WALK_SPEED,
                Mathf.Lerp(_animationHandler.GetFloat(AnimationParams.WALK_SPEED), _directionMagnitude,
                    Time.deltaTime * 4));

            SaveLastPosition();
        }

        private void RotatePlayer(Vector3 direction)
        {
            var targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            var angle = Mathf.LerpAngle(_model.RigidBody.transform.eulerAngles.y, targetAngle,
                10 * Time.deltaTime * _directionMagnitude);
            _model.RigidBody.MoveRotation(Quaternion.Euler(Vector3.up * angle));
        }

        #endregion

        private void SaveLastPosition() =>
            _model.PlayerStatsSo.LastPosition = _model.GO.transform.position;

        private float GetMovementSpeed =>
            _playerObservables.PlayerState switch
            {
                PlayerState.WALK => _model.BaseStatsSo.MovementSpeed,
                PlayerState.ATTACK =>
                    _model.BaseStatsSo.MovementSpeed * 0.9f,
                _ => _model.BaseStatsSo.MovementSpeed
            };


        public void Dispose()
        {
            _compositeDisposable?.Dispose();
        }


        [Serializable]
        public class UISettings
        {
            public Joystick Joystick;
        }
    }
}