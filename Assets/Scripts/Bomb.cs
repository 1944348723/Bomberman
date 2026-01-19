using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

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

        Explode();
        Destroy(this.gameObject);
    }

    private void Explode()
    {
        var explosionsContainer = GameManager.Instance.ExplosionsContainer;
        var cellPos = GameManager.Instance.indestructibles.WorldToCell(this.transform.position);
        Debug.Log(cellPos);
    }
}
