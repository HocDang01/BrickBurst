using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace CoreDang
{
    /// <summary>
    /// Used to sceneManager.LoadSceneAsync(...)
    /// Exp: await SceneManager.LoadSceneAsync("Game");
    /// await something; => Find something.GetAwaiter() => if has: IsCompleted, OnCompleted, GetResult
    /// </summary>
    public class AsyncOperationAwaiter : INotifyCompletion
    {
        private readonly AsyncOperation asyncOperator;
        private Action continuation;

        public AsyncOperationAwaiter(AsyncOperation asyncOp)
        {
            asyncOperator = asyncOp;
            asyncOperator.completed += OnOperatorCompleted;
        }

        public bool IsCompleted { get { return asyncOperator.isDone; } }

        public void GetResult() { }

        public void OnCompleted(Action continuation)
        {
            this.continuation = continuation;
        }

        private void OnOperatorCompleted(AsyncOperation asyncOp)
        {
            asyncOperator.completed -= OnOperatorCompleted;
            continuation?.Invoke();
        }
    }

    public static class AsyncOperationExtension
    {
        public static AsyncOperationAwaiter GetAwaiter(this AsyncOperation asyncOp)
        {
            return new AsyncOperationAwaiter(asyncOp);
        }
    }
}