using Constants;
using UnityEngine;
using Zenject;

namespace PlayerBehaviors
{
    public class AnimationHandler : IInitializable
    {
        #region INJECT

        private readonly BaseModel _baseModel;

        private AnimationHandler(BaseModel baseModel) => _baseModel = baseModel;

        #endregion


        public void Initialize()
        {
            _baseModel.GetAnimator.keepAnimatorControllerStateOnDisable = false;
        }

        public void Play(int anim, int layer, float normalizedTime = 0) =>
            _baseModel.GetAnimator.Play(anim, layer, normalizedTime);

        public void CrossFadeInFixed(int anim, int layer, float transitionDuration) =>
            _baseModel.GetAnimator.CrossFadeInFixedTime(anim, transitionDuration, layer, 0, 0);

        public float GetFloat(int param) => _baseModel.GetAnimator.GetFloat(param);

        public void SetFloat(int param, float speed) => _baseModel.GetAnimator.SetFloat(param, speed);


        public float GetAnimationInterval(int layer)
        {
            var normalizedTime = Mathf.Clamp01(1 - GetNormalizedTime(layer));
            var clipLength = GetLenght(layer);
            return clipLength * normalizedTime;
        }


        public float GetAnimationTransitionDuration(int layer) =>
            _baseModel.GetAnimator.GetAnimatorTransitionInfo(layer).duration;


        public float GetNormalizedTime(int layerIndex) =>
            _baseModel.GetAnimator.GetCurrentAnimatorStateInfo(layerIndex).normalizedTime;

        public void ResetAnimationState(float time = 0)
        {
            CrossFadeInFixed(AnimationClips.WALK, AnimationLayer.DEFAULT, time);
            CrossFadeInFixed(AnimationClips.NONE, AnimationLayer.UPPER, time);
        }

        #region HELPERS

        public float CalculateRemainedNormalizedTime(int animationLayer) =>
            1 - GetNormalizedTime(animationLayer);


        private float GetLenght(int layerIndex) =>
            _baseModel.GetAnimator.GetCurrentAnimatorStateInfo(layerIndex).length;

        #endregion
    }
}