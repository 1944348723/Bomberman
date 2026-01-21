using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] KeyCode MoveUpKey = KeyCode.UpArrow;
    [SerializeField] KeyCode MoveDownKey = KeyCode.DownArrow;
    [SerializeField] KeyCode MoveLeftKey = KeyCode.LeftArrow;
    [SerializeField] KeyCode MoveRightKey = KeyCode.RightArrow;
    [SerializeField] KeyCode DropBombKey = KeyCode.Space;

    private Bomberman bomberman;

    void Awake()
    {
        this.bomberman = GetComponent<Bomberman>();
        this.bomberman.OnDeath += () => this.enabled = false;
    }

    void Update()
    {
        if (Input.GetKey(MoveUpKey))
        {
            this.bomberman.Move(DirectionEnum.Up);
        } else if (Input.GetKey(MoveDownKey))
        {
            this.bomberman.Move(DirectionEnum.Down);
        } else if (Input.GetKey(MoveLeftKey))
        {
            this.bomberman.Move(DirectionEnum.Left);
        } else if (Input.GetKey(MoveRightKey))
        {
            this.bomberman.Move(DirectionEnum.Right);
        } else
        {
            this.bomberman.StopMove();
        }

        if (Input.GetKeyDown(DropBombKey))
        {
            this.bomberman.TryDropBomb();
        }
    }
}
