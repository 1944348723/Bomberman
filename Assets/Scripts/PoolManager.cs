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
    private Dictionary<string, ObjectPool<GameObject>> pools = new();
    public static PoolManager Instance { get; private set; }

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
                createFunc:() => Instantiate(config.prefab),
                actionOnGet: (obj) => obj.SetActive(true),
                actionOnRelease: (obj) => obj.SetActive(false)
            );
            pools.Add(config.name, pool);
        }
    }

    public GameObject Get(string name)
    {
        if (!pools.ContainsKey(name)) {
            Debug.LogError($"Pool {name} not found");
            return null;
        }

        return pools[name].Get();
    }

    public void Release(string name, GameObject obj) {
        if (!pools.ContainsKey(name)) {
            Debug.LogError($"Pool {name} not found");
            return;
        }

        pools[name].Release(obj);
    }
}
