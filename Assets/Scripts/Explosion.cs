using UnityEngine;

public class Explosion : MonoBehaviour
{
    // 这些动画一开始都是向右的
    [SerializeField] private AnimatedSpriteRenderer startAnimation;
    [SerializeField] private AnimatedSpriteRenderer middleAnimation;
    [SerializeField] private AnimatedSpriteRenderer endAnimation;
    [SerializeField] private string explosionPoolName = "Explosion";

    private AnimatedSpriteRenderer currentAnimation;

    void Awake()
    {
        this.currentAnimation = startAnimation;
        this.startAnimation.OnAnimationFinished += () =>  {
            this.transform.eulerAngles = new Vector3(0f, 0f, 0f);
            PoolManager.Instance.Release(this.explosionPoolName, this.gameObject);
        };
        this.middleAnimation.OnAnimationFinished += () => {
            this.transform.eulerAngles = new Vector3(0f, 0f, 0f);
            PoolManager.Instance.Release(this.explosionPoolName, this.gameObject);
        };
        this.endAnimation.OnAnimationFinished += () => {
            this.transform.eulerAngles = new Vector3(0f, 0f, 0f);
            PoolManager.Instance.Release(this.explosionPoolName, this.gameObject);
        };
    }

    // 只支持上下左右
    public void SetDirection(Vector3 direction)
    {
        if (direction == Vector3.up)
        {
            this.transform.eulerAngles = new Vector3(0f, 0f, 90f);
        } else if (direction == Vector3.down)
        {
            this.transform.eulerAngles = new Vector3(0f, 0f, -90f);
        } else if (direction == Vector3.left)
        {
            this.transform.eulerAngles = new Vector3(0f, 0f, 180f);
        }
    }

    public void PlayStart()
    {
        this.currentAnimation.enabled = false;
        this.currentAnimation = startAnimation;
        this.currentAnimation.enabled = true;
        this.currentAnimation.Play();
    }

    public void PlayMiddle()
    {
        this.currentAnimation.enabled = false;
        this.currentAnimation = middleAnimation;
        this.currentAnimation.enabled = true;
        this.currentAnimation.Play();
    }

    public void PlayEnd()
    {
        this.currentAnimation.enabled = false;
        this.currentAnimation = endAnimation;
        this.currentAnimation.enabled = true;
        this.currentAnimation.Play();
    }
}
