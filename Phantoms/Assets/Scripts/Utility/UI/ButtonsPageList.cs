using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class ButtonsPageList<T1, T2> : MonoBehaviour
{
    [Header("Button References")]
    [SerializeField]
    protected GameObject[] m_allButtons = null;
    [SerializeField]
    protected Button m_prevPageButton = null;
    [SerializeField]
    protected Button m_nextPageButton = null;

    protected int m_numPages = 0;
    protected int m_currentPage = 0;
    protected int m_itemsPerPage = 1;
    protected List<T2> m_dataList = new List<T2>();
    protected List<T1> m_buttonList = new List<T1>();

    private void Awake()
    {
        m_itemsPerPage = m_allButtons.Length;
        m_prevPageButton.gameObject.SetActive(false);
        m_nextPageButton.gameObject.SetActive(false);
        foreach (GameObject obj in m_allButtons)
        {
            m_buttonList.Add(obj.GetComponent<T1>());
        }
    }

    public virtual void SetList(List<T2> data)
    {
        m_dataList = data;
        m_numPages = (m_dataList.Count - 1) / m_itemsPerPage;
        if (m_numPages < 0) { m_numPages = 0; }
        if (m_numPages <= 0)
        {
            m_prevPageButton.gameObject.SetActive(false);
            m_nextPageButton.gameObject.SetActive(false);
        }
        else
        {
            m_prevPageButton.gameObject.SetActive(true);
            m_nextPageButton.gameObject.SetActive(true);
        }

        SetPage(0);
    }

    public void NextPage()
    {
        SetPage(m_currentPage + 1);
    }
    public void PrevPage()
    {
        SetPage(m_currentPage - 1);
    }
    public void FirstPage()
    {
        SetPage(0);
    }
    public void LastPage()
    {
        SetPage(m_numPages);
    }


    public virtual void SetPage(int pageNum)
    {
        pageNum = Mathf.Clamp(pageNum, 0, m_numPages);

        m_currentPage = pageNum;

        int startIndex = pageNum * m_itemsPerPage;

        for (int i = 0; i < m_itemsPerPage; i++)
        {
            int index = startIndex + i;
            if (index < m_dataList.Count)
            {
                m_allButtons[i].gameObject.SetActive(true);
                SetButton(m_buttonList[i], m_dataList[index]);
            }
            else
            {
                m_allButtons[i].gameObject.SetActive(false);
            }
        }

        if (pageNum <= 0)
        {
            m_prevPageButton.interactable = false;
        }
        else
        {
            m_prevPageButton.interactable = true;
        }
        if (pageNum >= m_numPages)
        {
            m_nextPageButton.interactable = false;
        }
        else
        {
            m_nextPageButton.interactable = true;
        }
    }

    protected abstract void SetButton(T1 Button, T2 Data);
}