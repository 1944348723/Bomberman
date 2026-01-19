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
            ObjectPool<GameObject> pool = new(() => Instantiate(config.prefab));
            pools.Add(config.name, pool);
        }
    }
}
