using UnityEngine;

public class Mover2D : MonoBehaviour
{
    public float speed = 5f;
    [SerializeField] Rigidbody2D rb;
    private Vector2 direction = Vector2.zero;

    // 复用，避免重复创建
    private Vector2 delta = new(0, 0);

    void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        Vector2 currentPosition = rb.position;
        delta.Set(direction.x * speed * Time.fixedDeltaTime, direction.y * speed * Time.fixedDeltaTime);
        this.rb.MovePosition(currentPosition + delta);
    }

    // 通过将方向设置为(0, 0)来停止移动
    public void SetDirection(Vector2 newDirection)
    {
        this.direction = newDirection;
    }

    public bool IsMoving()
    {
        return !this.direction.Equals(Vector2.zero);
    }
}
