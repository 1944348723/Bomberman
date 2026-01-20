using System.Collections;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] float explodeDelayTime = 3f;
    [SerializeField] GameObject explosionStartPrefab;
    [SerializeField] GameObject explosionMiddlePrefab;
    [SerializeField] GameObject explosionEndPrefab;

    private AnimatedSpriteRenderer anim;
    private Rigidbody2D rb;

    void Awake()
    {
        this.anim = GetComponent<AnimatedSpriteRenderer>();
        this.rb = GetComponent<Rigidbody2D>();
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
        var explosionsContainer = GameManager.Instance.ExplosionsContainer;
        var indestructibleWalls = GameManager.Instance.indestructibles;
        var destructibleWalls = GameManager.Instance.destructibles;

        var cellPos = GameManager.Instance.indestructibles.WorldToCell(this.transform.position);
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
    }
}
