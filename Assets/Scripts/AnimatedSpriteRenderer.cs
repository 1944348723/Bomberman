using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class AnimatedSpriteRenderer : MonoBehaviour
{
    [SerializeField] private Sprite[] frames;
    [SerializeField] private Sprite idleFrame;
    [SerializeField] private float frameDuration = 0.25f;
    [SerializeField] private bool loop = false;

    private SpriteRenderer sr;

    private int nextFrameIndex = 0;
    private float timer = 0;
    private bool playing = false;

    void Awake()
    {
        this.sr = this.GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        if (this.sr)
        {
            this.sr.enabled = true;
            this.sr.sprite = idleFrame;
        }
    }

    void OnDisable()
    {
        if (this.sr)
        {
            Stop();
            this.sr.enabled = false;
        }
    }

    void Update()
    {
        if (!playing) return;

        timer += Time.deltaTime;
        if (timer >= frameDuration)
        {
            timer -= frameDuration;
            bool isLastFrame = NextFrame();
            if (isLastFrame)
            {
                nextFrameIndex = 0;
                if (!loop)
                {
                    Stop();
                }
            }
        }
    }

    public void Play()
    {
        nextFrameIndex = 0;
        timer = 0;
        if (!sr || frames.Length == 0) return;

        playing = true;
        sr.sprite = frames[0];
        ++nextFrameIndex;
    }

    public void Stop()
    {
        playing = false;
        nextFrameIndex = 0;
        timer = 0;
        if (sr)
        {
            sr.sprite = idleFrame;
        }
    }

    // 返回是不是最后一帧了
    private bool NextFrame()
    {
        if (nextFrameIndex < 0 || nextFrameIndex >= frames.Length) return true;

        sr.sprite = frames[nextFrameIndex];
        ++nextFrameIndex;
        if (nextFrameIndex == frames.Length) return true;
        else return false;
    }
}
