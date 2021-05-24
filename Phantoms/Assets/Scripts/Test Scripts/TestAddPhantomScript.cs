using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAddPhantomScript : MonoBehaviour
{
    [Header("Save Stuff")]
        [SerializeField]
    private bool m_saveDisabledState = true;
    [SerializeField]
    private string m_phantomFlagName = "PhantomAdd_";
    

    [Header("Other things")]
    [SerializeField]
    private GameObject m_objectToDisable = null;

    [SerializeField]
    private string m_phantomID = null;

    [SerializeField]
    private PhantomData m_phantomData = null;

    [SerializeField]
    private int m_phantomLevel = 0;

    private bool m_triggered = false;

    private void Start()
    {
        if (m_objectToDisable == null)
        {
            m_objectToDisable = this.gameObject;
        }
        if (m_saveDisabledState && SaveDataManager.CheckFlag(m_phantomFlagName))
        {
            m_triggered = true;
            m_objectToDisable.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other) 
    {
        if (other.tag == "Player" && !m_triggered)
        {
            if (m_phantomData != null)
            {
                PlayerInventoryManager.Instance.AddPhantom(PhantomDataUtility.GenerateRandomPhantom(m_phantomData, m_phantomLevel));
            }
            else if (m_phantomID != null)
            {
                PlayerInventoryManager.Instance.AddPhantom(PhantomDataUtility.GenerateRandomPhantom(m_phantomID, m_phantomLevel));
            }

            if (m_saveDisabledState)
            {
                SaveDataManager.SetFlag(m_phantomFlagName);
            }

            m_objectToDisable.SetActive(false);
        }
    }
}
