// using System;
// using System.Runtime.CompilerServices;
// using UnityEngine.ResourceManagement.AsyncOperations;

// namespace CoreDang
// {
//     /// <summary>
//     /// Used to Addressables.LoadAssetAsync<T>()
//     /// var obj = await Addressables.LoadAssetAsync<GameObject>("key");
//     /// </summary>
//     /// <typeparam name="TObject"></typeparam>
//     public class AsyncOperationHandleAwaiter<TObject> : INotifyCompletion
//     {
//         private readonly AsyncOperationHandle<TObject> asyncOperator;
//         private Action continuation;

//         public AsyncOperationHandleAwaiter(AsyncOperationHandle<TObject> asyncOp)
//         {
//             asyncOperator = asyncOp;
//             asyncOperator.Completed += OnOperatorCompleted;
//         }

//         public bool IsCompleted { get { return asyncOperator.IsDone; } }

//         public TObject GetResult() => asyncOperator.Result;     // can get: var prefab = await handle;

//         public void OnCompleted(Action continuation)
//         {
//             this.continuation = continuation;
//         }

//         private void OnOperatorCompleted(AsyncOperationHandle<TObject> asyncOp)
//         {
//             asyncOperator.Completed -= OnOperatorCompleted;
//             continuation?.Invoke();
//         }
//     }

//     public static class AsyncOperationHandleExtension
//     {
//         /// <summary>
//         /// var obj = await Addressables.LoadAssetAsync<GameObject>("key"); Doesn't need .Task()
//         /// </summary>
//         /// <typeparam name="TObject"></typeparam>
//         /// <param name="asyncOp"></param>
//         /// <returns></returns>
//         public static AsyncOperationHandleAwaiter<TObject> GetAwaiter<TObject>(this AsyncOperationHandle<TObject> asyncOp)
//         {
//             return new AsyncOperationHandleAwaiter<TObject>(asyncOp);
//         }
//     }
// }