using UnityEngine;

public class Mover2D : MonoBehaviour
{
    public float speed = 5f;
    [SerializeField] Rigidbody2D rb;
    private Vector2 direction = new(0, 0);

    // 复用，避免重复创建
    private Vector2 delta = new(0, 0);

    void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody2D>();
    }

    public void Move(Vector2 direction)
    {
        this.direction = direction;
        Vector2 currentPosition = rb.position;
        delta.Set(direction.x * speed * Time.fixedDeltaTime, direction.y * speed * Time.fixedDeltaTime);
        this.rb.MovePosition(currentPosition + delta);
    }
}
