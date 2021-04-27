using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelicPickup : MonoBehaviour
{
    [Header("Data references")]
    [SerializeField]
    private string RelicID = null;
    public RelicData Relic = null;

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

        if (string.IsNullOrEmpty(RelicID) && Relic != null)
        {
            RelicID = Relic.ID;
        }

        SetRelic();
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Player")
        {
            PlayerInventoryManager.Instance.AddRelic(RelicID);
            this.gameObject.SetActive(false);
        }
    }

    // 
    public void SetRelic(string newRelicID)
    {
        RelicID = newRelicID;
        SetRelic();
    }

    private void SetRelic()
    {
        if (Relic == null)
        {
            Relic = DataManager.Instance.TryGetRelicData(RelicID);
            if (Relic == null) { return; }
        }
        m_materialCopy.mainTexture = Relic.Icon.texture;
    }
}
