using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public Transform bombsContainer;
    public Transform explosionsContainer;
    public Transform destructedWallsContainer;
    public Tilemap grass;
    public Tilemap indestructibles;
    public Tilemap destructibles;

    public static GameManager Instance { get; private set; }

    void Awake()
    {
        // 已经有了的话现在就相当于重复创建了，把当前的给销毁
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        // 跨场景的话
        DontDestroyOnLoad(gameObject);
    }
}
