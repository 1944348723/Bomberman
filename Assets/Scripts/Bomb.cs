using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Bomb : MonoBehaviour
{
    [SerializeField] private string bombPoolName = "Bomb";
    [SerializeField] private string bombLayerName = "Bomb";
    private Transform explosionsContainer;
    private Transform DestructedWallsContainer;
    private Tilemap indestructibleWalls;
    private Tilemap destructibleWalls;
    private Rigidbody2D rb;
    private CircleCollider2D selfCollider;

    private float explodeDelayTime = 3f;
    private int explosionLen = 1;
    private Coroutine explodeRoutine;
    private bool hasExploded = false;

    void Awake()
    {
        this.rb = GetComponent<Rigidbody2D>();
        this.selfCollider = GetComponent<CircleCollider2D>();
        this.explosionsContainer = GameManager.Instance.explosionsContainer;
        this.DestructedWallsContainer = GameManager.Instance.destructedWallsContainer;
        this.indestructibleWalls = GameManager.Instance.indestructibles;
        this.destructibleWalls = GameManager.Instance.destructibles;
    }

    public void Init(float delay, int len)
    {
        if (delay <= 0 || len <= 0) return;
        this.explodeDelayTime = delay;
        this.explosionLen = len;
        this.hasExploded = false;
        selfCollider.isTrigger = true;
        explodeRoutine = StartCoroutine(DelayExplode());
    }

    private IEnumerator DelayExplode()
    {
        yield return new WaitForSeconds(explodeDelayTime);

        this.explodeRoutine = null;
        Explode();
    }

    private void Explode()
    {
        if (this.hasExploded) return;
        this.hasExploded = true;
        // 定时还没到，是被提前引爆的，取消协程
        if (explodeRoutine != null)
        {
            StopCoroutine(explodeRoutine);
            this.explodeRoutine = null;
        }

        var cellPos = indestructibleWalls.WorldToCell(this.transform.position);
        // 中心
        if (!indestructibleWalls.HasTile(cellPos) && !destructibleWalls.HasTile(cellPos))
        {
            var explosionStart = PoolManager.Instance.Get("ExplosionStart");
            explosionStart.transform.SetParent(explosionsContainer);
            // -------------------!!!!!!!!!!!!!!!!!!!!!!--------------------------------------
            // SimulateMode为FixedUpdate时，FixedUpdate之前，会将Transform的位置同步到Rigidbody2D
            // 在FixedUpdate之后，会将Rigidbody2D的位置同步到Transform
            // 所以通过Rigidbody来设置位置只有在FixedUpdate中才有效，否则会被transform覆盖
            // 所以这里直接设置transform.position
            explosionStart.transform.position = new (Mathf.RoundToInt(this.rb.position.x), Mathf.RoundToInt(this.rb.position.y));
            // Physics2D.SyncTransforms();             // 手动将Transform的位置同步到物理系
        }
        SpreadExplosion(cellPos, Vector3Int.up, "ExplosionUpMiddle", "ExplosionUpEnd");
        SpreadExplosion(cellPos, Vector3Int.down, "ExplosionDownMiddle", "ExplosionDownEnd");
        SpreadExplosion(cellPos, Vector3Int.left, "ExplosionLeftMiddle", "ExplosionLeftEnd");
        SpreadExplosion(cellPos, Vector3Int.right, "ExplosionRightMiddle", "ExplosionRightEnd");
        
        PoolManager.Instance.Release(this.bombPoolName, this.gameObject);
    }

    private void SpreadExplosion(Vector3Int cell, Vector3Int direction, string middle, string end)
    {
        List<Vector3Int> explosionCells = new();
        Vector3Int currentCell = new(cell.x, cell.y);
        currentCell.x += direction.x;
        currentCell.y += direction.y;
        
        for (int i = 0; i < explosionLen; ++i)
        {
            Bomb bomb = null;
            if (indestructibleWalls.HasTile(currentCell)) break;
            if (destructibleWalls.HasTile(currentCell))
            {
                var destructedWall = PoolManager.Instance.Get("DestructedWall");
                destructedWall.GetComponent<AnimatedSpriteRenderer>().OnAnimationFinished += () => destructibleWalls.SetTile(currentCell, null);
                destructedWall.transform.SetParent(DestructedWallsContainer);
                destructedWall.transform.position = destructibleWalls.GetCellCenterWorld(currentCell);
                break;
            } else if (bomb = this.TryGetBomb(currentCell))
            {
                Debug.Log("Chain Explosion");
                // 连锁引爆
                bomb.Explode();
                break;
            } else
            {
                explosionCells.Add(currentCell);
                currentCell.x += direction.x;
                currentCell.y += direction.y;
            }
        }

        // 生成爆炸火焰
        for (int i = 0; i < explosionCells.Count; ++i)
        {
            if (i == explosionCells.Count - 1)
            {
                var explosion = PoolManager.Instance.Get(end);
                explosion.transform.SetParent(explosionsContainer);
                explosion.transform.position = indestructibleWalls.GetCellCenterWorld(explosionCells[i]);
            } else
            {
                var explosion = PoolManager.Instance.Get(middle);
                explosion.transform.SetParent(explosionsContainer);
                explosion.transform.position = indestructibleWalls.GetCellCenterWorld(explosionCells[i]);
            }
        }
    }

    private Bomb TryGetBomb(Vector3Int cell)
    {
        Bomb bomb = null;
        var worldPosition = indestructibleWalls.GetCellCenterWorld(cell);
        Collider2D hit = Physics2D.OverlapPoint(worldPosition, LayerMask.GetMask(this.bombLayerName));
        bomb = hit?.GetComponent<Bomb>();
        return bomb;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        this.GetComponent<CircleCollider2D>().isTrigger = false;
    }
}
