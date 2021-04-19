using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameHealthIndicator : MonoBehaviour
{
    [SerializeField]
    private Transform m_heartContainerParent = null;
    [SerializeField]
    private GameObject m_heartContainerPrefab = null;

    private List<Animator> m_heartContainers = new List<Animator>();
    private int m_currentHearts = 0;
    public int CurrentHearts { get { return m_currentHearts; } }
    private const string HURT_PARAMETER = "Hurt";

    public void SetMaxHearts(int numHearts)
    {
        int index = 0;
        while (index < numHearts)
        {
            if (m_heartContainers.Count <= index)
            {
                GameObject newHeartObject = Instantiate(m_heartContainerPrefab, m_heartContainerParent);
                m_heartContainers.Add(newHeartObject.GetComponent<Animator>());
            }
            else
            {
                m_heartContainers[index].gameObject.SetActive(true);
                m_heartContainers[index].SetBool(HURT_PARAMETER, false);
            }
            index++;
        }

        while (index < m_heartContainers.Count)
        {
            m_heartContainers[index].gameObject.SetActive(false);
            index++;
        }

        m_currentHearts = numHearts;
    }

    public void Damage(int damage = 1)
    {
        for (int i = 0; i < damage; i++)
        {
            int index = m_currentHearts - 1 - i;
            if (index >= 0)
            {
                m_heartContainers[m_currentHearts - 1 - i].SetBool(HURT_PARAMETER, true);
            }
        }

        m_currentHearts -= damage;
        if (m_currentHearts < 0) { m_currentHearts = 0; }
    }
}