using System.Collections;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private string bombPoolName = "Bomb";
    [SerializeField] private string bombLayerName = "Bomb";
    [SerializeField] private string explosionPoolName = "Explosion";

    public event System.Action OnBombExploded;

    private Rigidbody2D rb;
    private CircleCollider2D selfCollider;

    private float explodeDelayTime = 3f;
    private int explosionRadius = 1;
    private Coroutine explodeRoutine;
    private bool hasExploded = false;

    void Awake()
    {
        this.rb = GetComponent<Rigidbody2D>();
        this.selfCollider = GetComponent<CircleCollider2D>();
    }

    public void Init(float delay, int radius)
    {
        if (delay <= 0 || radius <= 0) return;
        this.explodeDelayTime = delay;
        this.explosionRadius = radius;
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

        Vector3 roundedPosition = new(Mathf.RoundToInt(this.rb.position.x), Mathf.RoundToInt(this.rb.position.y));
        // 中心
        var explosionStart = PoolManager.Instance.Get(this.explosionPoolName);
        explosionStart.GetComponent<Explosion>().PlayStart();
        // -------------------!!!!!!!!!!!!!!!!!!!!!!--------------------------------------
        // SimulateMode为FixedUpdate时，FixedUpdate之前，会将Transform的位置同步到Rigidbody2D
        // 在FixedUpdate之后，会将Rigidbody2D的位置同步到Transform
        // 所以通过Rigidbody来设置位置只有在FixedUpdate中才有效，否则会被transform覆盖
        // 所以这里直接设置transform.position
        explosionStart.transform.position = roundedPosition;
        // Physics2D.SyncTransforms();             // 手动将Transform的位置同步到物理系
        SpreadExplosion(roundedPosition, Vector3.up);
        SpreadExplosion(roundedPosition, Vector3.down);
        SpreadExplosion(roundedPosition, Vector3.left);
        SpreadExplosion(roundedPosition, Vector3.right);
        
        PoolManager.Instance.Release(this.bombPoolName, this.gameObject);
        this.OnBombExploded?.Invoke();
    }

    private void SpreadExplosion(Vector3 startPosition, Vector3 direction)
    {
        Vector3 currentPosition = new(startPosition.x + direction.x, startPosition.y + direction.y);
        
        for (int r = 1; r <= explosionRadius; ++r)
        {
            Bomb bomb = null;
            if (MapManager.Instance.HasIndestructibleWall(currentPosition)) break;
            else if (MapManager.Instance.HasDestructibleWall(currentPosition))
            {
                MapManager.Instance.DestrucWall(currentPosition);
                break;
            } else if (bomb = this.TryGetBomb(currentPosition))
            {
                // 连锁引爆
                bomb.Explode();
                break;
            } else
            {
                GameObject explosion = PoolManager.Instance.Get(this.explosionPoolName);
                var explosionComponent = explosion.GetComponent<Explosion>();
                explosionComponent.SetDirection(direction);
                explosion.transform.position = currentPosition;
                if (r == explosionRadius)
                {
                    explosionComponent.PlayEnd();
                } else
                {
                    explosionComponent.PlayMiddle();
                }
                currentPosition.x += direction.x;
                currentPosition.y += direction.y;
            }
        }
    }

    private Bomb TryGetBomb(Vector3 worldPosition)
    {
        Bomb bomb;
        Collider2D hit = Physics2D.OverlapPoint(worldPosition, LayerMask.GetMask(this.bombLayerName));
        bomb = hit?.GetComponent<Bomb>();
        return bomb;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        this.GetComponent<CircleCollider2D>().isTrigger = false;
    }
}
