using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace AddressableTools
{
    /// <summary>
    /// Вспомогательный класс для упрощения операций загрузки/выгрузки ресурсов
    /// </summary>
    public class AddressableLoader
    {
        public static T LoadAsset<T>(ComponentReference<T> reference) where T : Object
        {
            return reference.LoadAssetAsync().WaitForCompletion();
        }
        public static T LoadAsset<T>(AssetReferenceT<T> reference) where T : Object
        {
            return reference.LoadAssetAsync().WaitForCompletion();
        }
        public static T LoadAsset<T>(string addressablePath)
        {
            return Addressables.LoadAssetAsync<T>(addressablePath).WaitForCompletion();
        }
        public static AsyncOperationHandle<IList<T>> LoadAsset<T>(string addressablePath, List<T> collection)
        {
            var handle = Addressables.LoadAssetsAsync<T>(addressablePath, null);
            collection.AddRange(handle.WaitForCompletion());
;           return handle;
        }

        public static void LoadAssetSpiteSheet<T>(string addressablePath, List<T> collection)
        {
            AsyncOperationHandle<IList<T>> handle = Addressables.LoadAssetAsync<IList<T>>(addressablePath);
            handle.WaitForCompletion();
            collection.AddRange(handle.Result);
        }
        public static void LoadAssetSpiteSheet<T>(AssetReference reference, List<T> collection)
        {
            AsyncOperationHandle<IList<T>> handle = reference.LoadAssetAsync<IList<T>>();
            handle.WaitForCompletion();
            collection.AddRange(handle.Result);
        }
        public static void ReleaseAsset(string path)
        {
            Addressables.Release(path);
        }
        public static void ReleaseAsset<T>(AsyncOperationHandle<T> handle)
        {
            Addressables.Release(handle);
        }

        public static void ReleaseAssetList<T>(AsyncOperationHandle<IList<T>> handle)
        {
            Addressables.Release(handle);
        }
    }

}