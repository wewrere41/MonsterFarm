using System.Threading.Tasks;
using UnityEngine;

namespace Utilities.Extensions
{
    using System;
    using System.Runtime.InteropServices;
    using System.Threading;
    using Cysharp.Threading.Tasks;

    public static class TaskHelper
    {
        #region DELAY

        public static async UniTask<TResult> AddReturnDelay<TResult>(Func<TResult> getter, float seconds,
            [Optional] CancellationToken? cancellationToken)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(seconds),
                cancellationToken: cancellationToken ?? CancellationToken.None);
            return getter();
        }


        public static void AddMethodDelay(Action getter, float seconds, [Optional] CancellationToken? cancellationToken)
        {
            AsyncAddMethodDelay(getter, seconds, cancellationToken).Forget();
        }

        private static async UniTask AsyncAddMethodDelay(Action getter, float seconds,
            [Optional] CancellationToken? cancellationToken)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(seconds),
                cancellationToken: cancellationToken ?? CancellationToken.None);
            getter.Invoke();
        }

        public static async Task FuncTask(Func<bool> func)
        {
            while (!func())
            {
                await Task.Delay(1);
            }

            await Task.Delay(5);
            await Task.Delay(TimeSpan.FromSeconds(Time.deltaTime));
        }

        public static async Task FuncTask(Func<bool> func, CancellationTokenSource cancellationTokenSource)
        {
            while (!cancellationTokenSource.IsCancellationRequested && !func())
            {
                await Task.Delay(1);
            }

            await Task.Delay(5);
            await Task.Delay(TimeSpan.FromSeconds(Time.deltaTime));
        }

        #endregion
    }
}