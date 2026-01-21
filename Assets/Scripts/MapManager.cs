using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    public Tilemap indestructibleWalls;
    public Tilemap destructibleWalls;
    [SerializeField] private string destructedWallPoolName;

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

    public void SpawnDestructedWall(Vector3 worldPosition)
    {
        destructibleWalls.SetTile(destructibleWalls.WorldToCell(worldPosition), null);

        var destructedWall = PoolManager.Instance.Get(this.destructedWallPoolName);
        destructedWall.transform.position = worldPosition;
    }
}
