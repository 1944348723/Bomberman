using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Bomb : MonoBehaviour
{
    private Transform explosionsContainer;
    private Transform DestructedWallsContainer;
    private Tilemap indestructibleWalls;
    private Tilemap destructibleWalls;
    private AnimatedSpriteRenderer anim;
    private Rigidbody2D rb;

    private float explodeDelayTime = 3f;
    private int explosionLen = 1;

    void Awake()
    {
        this.anim = GetComponent<AnimatedSpriteRenderer>();
        this.rb = GetComponent<Rigidbody2D>();
        this.explosionsContainer = GameManager.Instance.explosionsContainer;
        this.DestructedWallsContainer = GameManager.Instance.destructedWallsContainer;
        this.indestructibleWalls = GameManager.Instance.indestructibles;
        this.destructibleWalls = GameManager.Instance.destructibles;
    }

    void Start()
    {
        this.anim.Play();
        StartCoroutine(DelayExplode());
    }

    private IEnumerator DelayExplode()
    {
        yield return new WaitForSeconds(explodeDelayTime);

        Explode();
        Destroy(this.gameObject);
    }

    private void Explode()
    {

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
            explosionStart.transform.position = this.rb.position;
            // Physics2D.SyncTransforms();             // 手动将Transform的位置同步到物理系统
        }
        SpreadExplosion(cellPos, Vector3Int.up, "ExplosionUpMiddle", "ExplosionUpEnd");
        SpreadExplosion(cellPos, Vector3Int.down, "ExplosionDownMiddle", "ExplosionDownEnd");
        SpreadExplosion(cellPos, Vector3Int.left, "ExplosionLeftMiddle", "ExplosionLeftEnd");
        SpreadExplosion(cellPos, Vector3Int.right, "ExplosionRightMiddle", "ExplosionRightEnd");
    }

    private void SpreadExplosion(Vector3Int cell, Vector3Int direction, string middle, string end)
    {
        List<Vector3Int> explosionCells = new();
        Vector3Int currentCell = new(cell.x, cell.y);
        currentCell.x += direction.x;
        currentCell.y += direction.y;
        
        for (int i = 0; i < explosionLen; ++i)
        {
            if (indestructibleWalls.HasTile(currentCell)) break;
            if (destructibleWalls.HasTile(currentCell))
            {
                var destructedWall = PoolManager.Instance.Get("DestructedWall");
                destructedWall.GetComponent<AnimatedSpriteRenderer>().OnAnimationFinished += () => destructibleWalls.SetTile(currentCell, null);
                destructedWall.transform.SetParent(DestructedWallsContainer);
                destructedWall.transform.position = destructibleWalls.GetCellCenterWorld(currentCell);
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
}
