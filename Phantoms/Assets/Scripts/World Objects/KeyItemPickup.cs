using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyItemPickup : MonoBehaviour
{
    [Header("Data references")]
    [SerializeField]
    private string ItemID = null;
    public KeyItemData KeyItem = null;

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

        if (string.IsNullOrEmpty(ItemID) && KeyItem != null)
        {
            ItemID = KeyItem.ItemID;
        }

        SetItem();
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player")
        {
            PlayerInventoryManager.Instance.AddKeyItem(ItemID);
            AudioManager.PlaySound(m_itemPickupSoundEffect);
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
        if (KeyItem == null)
        {
            KeyItem = DataManager.Instance.TryGetKeyItemData(ItemID);
            if (KeyItem == null) { return; }
        }
        m_materialCopy.mainTexture = KeyItem.Sprite.texture;
    }
}
