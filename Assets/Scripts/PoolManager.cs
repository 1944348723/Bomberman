using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

[System.Serializable]
public struct PoolConfig
{
    public string name;
    public GameObject prefab;
};


/*
    封装了Unity的对象池，只存储Prefab，对象池中存放的一般也就是Prefab，能满足大多数场景需求

    通过在Inspector中填入池的名字和对应Prefab来配置对象池，后续都是通过此处填入的名字来访问对象池

    在创建对象池的同时在PoolManager所在GameObject下为每一个池都创建了一个GameObject作为该对象池中对象的父节点
    命名为config.name + "Container"，方便在运行时直观的观察和调试
*/
public class PoolManager : MonoBehaviour
{
    [SerializeField] private PoolConfig[] poolConfigs;
    public static PoolManager Instance { get; private set; }

    private Dictionary<string, ObjectPool<GameObject>> pools = new();
    private Dictionary<string, Transform> containers = new();

    void Awake()
    {
        // 已经有了的话现在就相当于重复创建了，把当前的给销毁
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        // 跨场景的话
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (poolConfigs.Length <= 0) return;

        foreach (PoolConfig config in poolConfigs) {
            ObjectPool<GameObject> pool = new(
                createFunc:() => Instantiate(config.prefab)
            );
            pools.Add(config.name, pool);

            GameObject container = new($"{config.name}Container");
            container.transform.SetParent(transform);
            containers.Add(config.name, container.transform);
        }
    }

    public GameObject Get(string name)
    {
        if (!pools.ContainsKey(name)) {
            Debug.LogError($"Pool {name} not found");
            return null;
        }

        var obj = pools[name].Get();
        obj.SetActive(true);
        return obj;
    }

    public void Release(string name, GameObject obj) {
        if (!pools.ContainsKey(name)) {
            Debug.LogError($"Pool {name} not found");
            return;
        }

        obj.SetActive(false);
        obj.transform.SetParent(containers[name]);
        pools[name].Release(obj);
    }
}
