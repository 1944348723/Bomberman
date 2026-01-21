using System.Linq.Expressions;
using UnityEngine;

public enum DirectionEnum { Up, Down, Left, Right };
public static class DirectionEnumExtension
{
    public static Vector2 ToVector2(this DirectionEnum direction)
    {
        switch (direction)
        {
            case DirectionEnum.Up: return Vector2.up;
            case DirectionEnum.Down: return Vector2.down;
            case DirectionEnum.Left: return Vector2.left;
            case DirectionEnum.Right: return Vector2.right;
        }
        return Vector2.zero;
    }
}


public class Bomberman : MonoBehaviour
{
    [SerializeField] private AnimatedSpriteRenderer upAnimation;
    [SerializeField] private AnimatedSpriteRenderer downAnimation;
    [SerializeField] private AnimatedSpriteRenderer leftAnimation;
    [SerializeField] private AnimatedSpriteRenderer rightAnimation;
    [SerializeField] private AnimatedSpriteRenderer deathAnimation;
    [SerializeField] private string bombPoolName;
    [SerializeField] private int explosionLayer;

    public event System.Action OnDeath;
    private Mover2D mover;
    private DirectionEnum currentDirection = DirectionEnum.Down;
    private AnimatedSpriteRenderer currentAnimation;
    private float explosionDelay = 3f;
    private int explosionLen = 1;

    void Awake()
    {
        this.mover = GetComponent<Mover2D>();

        currentAnimation = downAnimation;
        currentAnimation.enabled = true;
    }

    public void Move(DirectionEnum newDir)
    {
        // 动画
        if (newDir != currentDirection || !mover.IsMoving())
        {
            this.currentAnimation.enabled = false;

            this.currentDirection = newDir;
            currentAnimation = GetAnimation(currentDirection);
            this.currentAnimation.enabled = true;
            this.currentAnimation.Play();
        }

        // 移动
        this.mover.SetDirection(newDir.ToVector2());
    }

    public void StopMove()
    {
        this.mover.SetDirection(Vector2.zero);
        this.currentAnimation.Stop();
    }

    public void DropBomb()
    {
        GameObject bomb = PoolManager.Instance.Get(this.bombPoolName);
        bomb.transform.SetParent(GameManager.Instance.bombsContainer);
        Vector2 position = new Vector2(Mathf.Round(this.transform.position.x), Mathf.Round(this.transform.position.y));
        bomb.GetComponent<Rigidbody2D>().position = position;
        bomb.transform.position = position;
        bomb.GetComponent<Bomb>().Init(this.explosionDelay, this.explosionLen);
    }

    private AnimatedSpriteRenderer GetAnimation(DirectionEnum direction)
    {
        switch (direction)
        {
            case DirectionEnum.Up: return upAnimation;
            case DirectionEnum.Down: return downAnimation;
            case DirectionEnum.Left: return leftAnimation;
            case DirectionEnum.Right: return rightAnimation;
        }
        return downAnimation;
    }

    private void Die()
    {
        this.StopMove();
        this.currentAnimation.enabled = false;
        this.deathAnimation.enabled = true;
        this.deathAnimation.Play();
        this.deathAnimation.OnAnimationFinished += () => Destroy(this.gameObject);
        this.OnDeath?.Invoke();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == this.explosionLayer)
        {
            this.Die();
        }
    }
}
