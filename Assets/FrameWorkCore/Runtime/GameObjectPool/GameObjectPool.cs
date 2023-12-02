using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectPool : MonoBehaviour
{
    public class PoolInfo
    {
        public string path;
        public GameObjectPoolTag tag;

        private ResLoader resLoader;
        public List<GameObject> pool;
        public GameObject prefab;

        public PoolInfo(string path, GameObjectPoolTag tag)
        {
            this.path = path;
            this.tag = tag;
            pool = new List<GameObject>();
            resLoader = new ResLoader();
            prefab = resLoader.LoadAsset<GameObject>(path);
            if(prefab == null)
            {
                Debug.LogError($"加载预制体失败:{path}");
            }
        }

        public GameObject GetGameObject()
        {
            return Instantiate(prefab);
        }

        public void RecycleGameObject(GameObject go)
        {
            go.SetActive(false);
            pool.Add(go);
        }

        public void Release()
        {
            foreach(var go in pool)
            {
                Destroy(go);
            }
            resLoader.Dispose();
            resLoader = null;
        }
    }

    public static GameObjectPool Instance;
    public Dictionary<string, PoolInfo> dicPath2Info;
    public Dictionary<GameObjectPoolTag, List<PoolInfo>> dicTag2ListPool;
    public Dictionary<int, PoolInfo> dicId2Pool;

    private void Awake()
    {
        Instance = this;
        dicPath2Info = new Dictionary<string, PoolInfo>();
        dicTag2ListPool = new Dictionary<GameObjectPoolTag, List<PoolInfo>>();
        dicId2Pool = new Dictionary<int, PoolInfo>();
    }

    public GameObject GetGameObject(string path, GameObjectPoolTag tag = GameObjectPoolTag.None)
    {
        if (!dicPath2Info.ContainsKey(path))
        {
            PoolInfo poolInfo = new PoolInfo(path, tag);
            dicPath2Info.Add(path, poolInfo);
            if (!dicTag2ListPool.ContainsKey(tag))
            {
                dicTag2ListPool.Add(tag, new List<PoolInfo>());
            }
            dicTag2ListPool[tag].Add(poolInfo);
        }
        var go = dicPath2Info[path].GetGameObject();
        go.transform.SetParent(this.transform);
        return go;
    }

    public void Recycle(GameObject go)
    {
        if (dicId2Pool.ContainsKey(go.GetInstanceID()))
        {
            go.transform.SetParent(transform);
            dicId2Pool[go.GetInstanceID()].RecycleGameObject(go);
            dicId2Pool.Remove(go.GetInstanceID());
        }
        else
        {
            Destroy(go);
        }
    }

    public void ReleasePool(string path)
    {
        PoolInfo poolInfo;
        dicPath2Info.TryGetValue(path, out poolInfo);

        if(poolInfo != null)
        {
            ReleasePool(poolInfo);
        }
    }

    public void ReleasePool(GameObjectPoolTag tag)
    {
        List<PoolInfo> poolInfos;
        dicTag2ListPool.TryGetValue(tag, out poolInfos);
        if(poolInfos != null)
        {
            var tempPoolInfos = UnityEngine.Pool.ListPool<PoolInfo>.Get();
            tempPoolInfos.AddRange(poolInfos);
            for(int i = tempPoolInfos.Count - 1; i >= 0; --i)
            {
                ReleasePool(tempPoolInfos[i]);
            }
            UnityEngine.Pool.ListPool<PoolInfo>.Release(tempPoolInfos);
            dicTag2ListPool.Remove(tag);
        }
    }

    private void ReleasePool(PoolInfo poolInfo)
    {
        poolInfo.Release();
        dicPath2Info.Remove(poolInfo.path);
        dicTag2ListPool[poolInfo.tag].Remove(poolInfo);
    }
}