using System.Collections;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] float explodeDelayTime = 3f;
    private AnimatedSpriteRenderer anim;

    void Awake()
    {
        this.anim = GetComponent<AnimatedSpriteRenderer>();
        Debug.Log(this.anim);
    }

    void Start()
    {
        Debug.Log("Start");
        this.anim.Play();
        StartCoroutine(DelayExplode());
    }

    private IEnumerator DelayExplode()
    {
        yield return new WaitForSeconds(explodeDelayTime);

        Destroy(this.gameObject);
    }
}
