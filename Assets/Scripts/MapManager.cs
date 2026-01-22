using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
struct ItemConfig
{
    public ItemPickup item;
    [Range(0, float.MaxValue)]
    public float weight;
}

public class MapManager : MonoBehaviour
{
    public Tilemap indestructibleWalls;
    public Tilemap destructibleWalls;
    [SerializeField] private string destructedWallPoolName;
    [SerializeField] private ItemConfig[] items;
    [Range(0, 1)]
    [SerializeField] private float itemDropRate = 0.1f;
    private float totalWeight = 0;

    public static MapManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        foreach(var item in items)
        {
            totalWeight += item.weight;
        }
    }

    public bool HasIndestructibleWall(Vector3 worldPosition)
    {
        Vector3Int cell = this.indestructibleWalls.WorldToCell(worldPosition);
        return indestructibleWalls.HasTile(cell);
    }

    public bool HasDestructibleWall(Vector3 worldPosition)
    {
        Vector3Int cell = this.destructibleWalls.WorldToCell(worldPosition);
        return destructibleWalls.HasTile(cell);
    }

    public void DestrucWall(Vector3 worldPosition)
    {
        // 清除原来的墙
        destructibleWalls.SetTile(destructibleWalls.WorldToCell(worldPosition), null);

        // 创建墙被炸毁的动画
        var destructedWall = PoolManager.Instance.Get(this.destructedWallPoolName);
        destructedWall.transform.position = worldPosition;

        // 生成道具
        if (Random.Range(0, 1) < this.itemDropRate)
        {
            SpawnRandomItem(worldPosition);
        }
    }

    private void SpawnRandomItem(Vector3 worldPosition)
    {
        float r = Random.Range(0, totalWeight);

        float acc = 0;
        foreach (var itemConfig in items)
        {
            acc += itemConfig.weight;
            if (r < acc)
            {
                GameObject item = Instantiate(itemConfig.item.gameObject);
                item.transform.position = worldPosition;
                break;
            }
        }
    }
}
