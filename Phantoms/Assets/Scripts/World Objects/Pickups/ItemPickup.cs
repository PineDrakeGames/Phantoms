using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    [Header("Data references")]
    [SerializeField]
    private string ItemID = null;
    public ItemData Item = null;

    [Header("Object References")]
    [SerializeField]
    private Renderer m_itemRenderer = null;

    [SerializeField]
    private AudioClip m_itemPickupSoundEffect;

    private Material m_materialCopy = null;

    private void Awake()
    {
        if (m_itemRenderer == null)
        {
            m_itemRenderer = GetComponentInChildren<Renderer>();
        }
        m_materialCopy = new Material(m_itemRenderer.material);
        m_itemRenderer.material = m_materialCopy;

        if (string.IsNullOrEmpty(ItemID) && Item != null)
        {
            ItemID = Item.ItemID;
        }

        SetItem();
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player")
        {
            PlayerInventoryManager.Instance.AddItem(ItemID);
            AudioManager.PlaySound(m_itemPickupSoundEffect);

            string notification = "You discovered an item: <b>" + Item.AresData.DisplayName + "</b>!\n" + Item.AresData.Description;
            NotificationManager.SetBottomNotification(Item.Sprite, notification);

            this.gameObject.SetActive(false);
        }
    }

    // 
    public void SetItem(string newItemID)
    {
        ItemID = newItemID;
        Item = DataManager.Instance.TryGetItemData(ItemID);
        SetItem();
    }

    private void SetItem()
    {
        if (Item == null)
        {
            Item = DataManager.Instance.TryGetItemData(ItemID);
            if (Item == null) { return; }
        }
        m_materialCopy.mainTexture = Item.Sprite.texture;
    }
}
