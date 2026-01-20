using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(SpriteRenderer))]
public class AnimatedSpriteRenderer : MonoBehaviour
{
    [SerializeField] private Sprite[] frames;
    [SerializeField] private Sprite idleFrame;
    [SerializeField] private float frameDuration = 0.25f;
    [SerializeField] private bool loop = false;
    [SerializeField] private bool playOnEnable = false;
    [SerializeField] private bool destroyOnFinished = false;

    private SpriteRenderer sr;

    private int nextFrameIndex = 0;
    private float timer = 0;
    private bool playing = false;
    private bool finished = false;  // 只有正在播放，并且没有播放完才为true

    void Awake()
    {
        this.sr = this.GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        if (!this.sr) return;

        this.sr.enabled = true;
        this.sr.sprite = idleFrame;
        if (playOnEnable) this.Play();
    }

    void OnDisable()
    {
        if (this.sr)
        {
            this.sr.enabled = false;
            Stop();
        }
    }

    void Update()
    {
        if (!playing) return;
        if (finished) {
            Stop();
            finished = false;
            if (destroyOnFinished) Destroy(gameObject);
            return;
        }

        timer += Time.deltaTime;
        while (timer >= frameDuration)
        {
            timer -= frameDuration;
            bool isLastFrame = NextFrame();
            if (!isLastFrame) continue;

            Assert.IsTrue(isLastFrame);
            nextFrameIndex = 0;
            if (!loop) finished = true;
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
