using UnityEngine;

public class AnimationAutoReleaser : MonoBehaviour
{
    [SerializeField] string poolName;

    void Awake()
    {
        var animatedSpriteRenderer = GetComponent<AnimatedSpriteRenderer>();
        animatedSpriteRenderer.OnAnimationFinished += () => PoolManager.Instance.Release(poolName, this.gameObject);
    }
}
