using UnityEngine;

public class Bomberman : MonoBehaviour
{
    private Mover2D mover;

    void Awake()
    {
        this.mover = GetComponent<Mover2D>();
    }

    public void MoveUp()
    {
        this.mover.Move(Vector2.up);
    }

    public void MoveDown()
    {
        this.mover.Move(Vector2.down);
    }

    public void MoveLeft()
    {
        this.mover.Move(Vector2.left);
    }

    public void MoveRight()
    {
        this.mover.Move(Vector2.right);
    }

    public void DropBomb()
    {
        
    }
}
