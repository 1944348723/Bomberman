using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [SerializeField] private string bombermanLayerName = "Bomberman";
    enum ItemType
    {
        ExtraBomb,
        BlastRadius,
        SpeedIncrease
    };

    [SerializeField] private ItemType itemType;

    void OnTriggerEnter2D(Collider2D otherCollider)
    {
        if (otherCollider.gameObject.layer == LayerMask.NameToLayer(this.bombermanLayerName))
        {
            OnPickup(otherCollider.GetComponent<Bomberman>());
        }
    }

    private void OnPickup(Bomberman bomberman)
    {
        switch (this.itemType)
        {
            case ItemType.ExtraBomb:
                bomberman.AddBomb();
                break;
            case ItemType.BlastRadius:
                bomberman.BlastRadius();
                break;
            case ItemType.SpeedIncrease:
                bomberman.IncreaseSpeed();
                break;
        }
        Destroy(this.gameObject);
    }
}
