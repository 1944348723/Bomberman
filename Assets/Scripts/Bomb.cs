using System.Collections;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] float explodeDelayTime = 3f;
    [SerializeField] GameObject explosionStart;
    [SerializeField] GameObject explosionMiddle;
    [SerializeField] GameObject explosionEnd;

    private AnimatedSpriteRenderer anim;

    void Awake()
    {
        this.anim = GetComponent<AnimatedSpriteRenderer>();
    }

    void Start()
    {
        this.anim.Play();
        StartCoroutine(DelayExplode());
    }

    private IEnumerator DelayExplode()
    {
        yield return new WaitForSeconds(explodeDelayTime);

        Destroy(this.gameObject);
    }

    private void Explode()
    {

    }
}
