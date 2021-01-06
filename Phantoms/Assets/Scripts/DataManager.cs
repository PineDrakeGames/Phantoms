using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    private const string DATA_MANAGER_PREFAB = "Data Manager";

    private static DataManager s_instance = null;
    public static DataManager Instance
    {
        get
        {
            if (s_instance == null)
            {
                GameObject instance = Instantiate(Resources.Load(DATA_MANAGER_PREFAB, typeof(GameObject))) as GameObject;
                s_instance = instance.GetComponent<DataManager>();
                if (s_instance)
                {
                    s_instance.Initialize();
                }
            }
            return s_instance;
        }
    }

    [SerializeField]
    private PhantomDataTable m_phantomData = null;
    public static PhantomDataTable PhantomData
    {
        get { return Instance.m_phantomData; }
    }

    private Dictionary<string, PhantomData> m_IdToData = null;

    public void Initialize()
    {
        // Any initialization things
        DontDestroyOnLoad(this.gameObject);

        if (m_IdToData == null)
        {
            m_IdToData = new Dictionary<string, PhantomData>();
            foreach (PhantomData data in PhantomData.Data)
            {
                m_IdToData.Add(data.PhantomID, data);
            }
        }
    }

    public PhantomData TryGetPhantomData(string phantomID)
    {
        if (m_IdToData.ContainsKey(phantomID))
        {
            return m_IdToData[phantomID];
        }
        else
        {
            return null;
        }
    }
}
