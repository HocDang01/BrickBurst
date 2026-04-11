// using System.Threading.Tasks;
// using UnityEngine;
// using UnityEngine.AddressableAssets;
// public static class AssetReferenceExt
// {
//     public static async Task<T> GetOrLoadAsync<T>(this AssetReference assetReference)
//     {
//         if(assetReference.OperationHandle.IsValid() && assetReference.OperationHandle.Result != null)
//             return (T) assetReference.OperationHandle.Result;
//         else
//             return await assetReference.LoadAssetAsync<T>().Task;

//     }

//     public static async Task<GameObject> GetOrLoadAsync(this AssetReference assetReference)
//     {
//         if (assetReference.OperationHandle.IsValid() && assetReference.OperationHandle.Result != null)
//             return assetReference.OperationHandle.Result as GameObject;
//         else
//             return await assetReference.LoadAssetAsync<GameObject>().Task;
//     }
// }

