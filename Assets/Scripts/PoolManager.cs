using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

[System.Serializable]
public struct PoolConfig
{
    public string name;
    public GameObject prefab;
};

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
