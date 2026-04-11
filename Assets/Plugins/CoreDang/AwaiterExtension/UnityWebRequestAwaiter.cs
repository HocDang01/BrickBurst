using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Networking;

namespace CoreDang
{
    /// <summary>
    /// var request = UnityWebRequest.Get(url);
    /// await request.SendWebRequest();
    /// Debug.Log(request.downloadHandler.text);
    /// </summary>
    public class UnityWebRequestAwaiter : INotifyCompletion
    {
        private readonly UnityWebRequestAsyncOperation asyncOperator;
        private Action continuation;

        public UnityWebRequestAwaiter(UnityWebRequestAsyncOperation asyncOp)
        {
            asyncOperator = asyncOp;
            asyncOp.completed += OnOperatorCompleted;
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

    public static class UnityWebRequestExtension
    {
        public static UnityWebRequestAwaiter GetAwaiter(this UnityWebRequestAsyncOperation asyncOp)
        {
            return new UnityWebRequestAwaiter(asyncOp);
        }
    }
}