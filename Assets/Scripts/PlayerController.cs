using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] KeyCode MoveUpKey = KeyCode.UpArrow;
    [SerializeField] KeyCode MoveDownKey = KeyCode.DownArrow;
    [SerializeField] KeyCode MoveLeftKey = KeyCode.LeftArrow;
    [SerializeField] KeyCode MoveRightKey = KeyCode.RightArrow;

    private Bomberman bomberman;

    void Awake()
    {
        this.bomberman = GetComponent<Bomberman>();
    }

    void Update()
    {
        if (Input.GetKey(MoveUpKey))
        {
            this.bomberman.MoveUp();
        } else if (Input.GetKey(MoveDownKey))
        {
            this.bomberman.MoveDown();
        } else if (Input.GetKey(MoveLeftKey))
        {
            this.bomberman.MoveLeft();
        } else if (Input.GetKey(MoveRightKey))
        {
            this.bomberman.MoveRight();
        }
    }
}
