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
            this.gameObject.SetActive(false);
        }
    }

    // 
    public void SetItem(string newItemID)
    {
        ItemID = newItemID;
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
