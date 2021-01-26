using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAddPhantomScript : MonoBehaviour
{
    [SerializeField]
    private string m_phantomID = null;

    [SerializeField]
    private PhantomData m_phantomData = null;

    [SerializeField]
    private int m_phantomLevel = 0;

    private void OnTriggerEnter(Collider other) 
    {
        if (other.tag == "Player")
        {
            if (m_phantomData != null)
            {
                PlayerInventoryManager.Instance.AddPhantom(PhantomDataUtility.GenerateRandomPhantom(m_phantomData, m_phantomLevel));
            }
            else if (m_phantomID != null)
            {
                PlayerInventoryManager.Instance.AddPhantom(PhantomDataUtility.GenerateRandomPhantom(m_phantomID, m_phantomLevel));
            }

            gameObject.SetActive(false);
        }
    }
}
