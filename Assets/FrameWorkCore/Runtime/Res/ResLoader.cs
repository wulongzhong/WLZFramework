using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ResLoader : IDisposable
{
    private bool disposed = true;
    Dictionary<string, UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle> dicCache = new Dictionary<string, UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationHandle>();

    public T LoadAsset<T>(string path) where T : UnityEngine.Object
    {
        if (dicCache.ContainsKey(path))
        {
            return (T)dicCache[path].Result;
        }

        var handle = Addressables.LoadAssetAsync<T>(path);
        handle.WaitForCompletion();
        dicCache.Add(path, handle);
        return handle.Result;
    }

    public void Dispose()
    {
        Dispose(true);

        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                foreach(var v in dicCache.Values)
                {
                    Addressables.Release(v);
                }
            }
            disposed = true;
        }
    }
    ~ResLoader()
    {
        Dispose(false);
    }
}
