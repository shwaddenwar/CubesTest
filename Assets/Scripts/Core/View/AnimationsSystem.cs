using System;
using DG.Tweening;
using UnityEngine;

namespace Core.View {
    public static class AnimationsSystem {
        private const float JumpTime = 0.8f;
        private const float JumpStartTime = 0.3f;
        private const float JumpSpeed = 400;
        private const int JumpPunchVibrato = 2;
        private const float JumpPunchElasticity = 2f;
        private const float DestroyTime = 0.8f;
        private const float FloatToTime = 0.3f;
        private const float HoleFloatTime = 0.3f;
        private const float HoleFallTime = 0.5f;

        public static async void JumpToTower(RectTransform cubeTransform, Vector2 toPosition,
            RectTransform cubeViewTransform, Action onDone = null) {
            var jumpPunch = new Vector2(1.2f, 0.4f);
            var jumpDuration = Mathf.Min((toPosition - (Vector2)cubeTransform.localPosition).y / JumpSpeed, 0.6f);
            cubeViewTransform.DOPunchScale(jumpPunch, JumpStartTime, JumpPunchVibrato, JumpPunchElasticity);
            await cubeTransform.DOJump(toPosition, 0.1f, 1, jumpDuration).SetDelay(JumpStartTime - 0.1f)
                .AsyncWaitForCompletion();
            onDone?.Invoke();
        }

        public static async void DestroyCube(RectTransform cubeTransform, Action onDone = null) {
            cubeTransform.DOKill();
            cubeTransform.DOLocalRotate(new Vector3(0, 0, 180), DestroyTime);
            await cubeTransform.DOScale(Vector3.zero, DestroyTime).AsyncWaitForCompletion();
            onDone?.Invoke();
        }

        public static async void FloatTo(RectTransform cubeTransform, Vector2 toPosition, Action onDone = null) {
            cubeTransform.DOKill();
            await cubeTransform.DOMove(toPosition, FloatToTime).AsyncWaitForCompletion();
            onDone?.Invoke();
        }

        public static async void DropIntoHole(RectTransform cubeTransform,
            Transform aboveHoleTransform,
            Transform fallToTransform,
            Transform holeMaskTransform, Action onDone = null) {
            var sequence = DOTween.Sequence();
            sequence.Append(cubeTransform.DOMove(aboveHoleTransform.position, HoleFloatTime));
            sequence.AppendCallback(() => cubeTransform.SetParent(holeMaskTransform, true));
            sequence.Append(cubeTransform.DOMove(fallToTransform.position, HoleFallTime));
            sequence.Join(cubeTransform.DOLocalRotate(new Vector3(0, 0, 180), HoleFallTime));

            await sequence.AsyncWaitForCompletion();
            onDone?.Invoke();
        }
    }
}