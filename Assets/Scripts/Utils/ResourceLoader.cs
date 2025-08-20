using System;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Utils {
    public static class ResourceLoader {
        public static T LoadResource<T>(string path) {
            var loadOperation = Addressables.LoadAssetAsync<T>(path);
            return loadOperation.WaitForCompletion();
        }

        public static void LoadResourceAsync<T>(string path, Action<T> onLoaded) {
            if (onLoaded is null) {
                return;
            }

            var loadOperation = Addressables.LoadAssetAsync<T>(path);
            loadOperation.Completed += (op) => OnAssetLoaded<T>(op, onLoaded);
        }

        private static void OnAssetLoaded<T>(AsyncOperationHandle<T> op, Action<T> onLoaded) {
            onLoaded.Invoke(op.Result);
        }
    }
}