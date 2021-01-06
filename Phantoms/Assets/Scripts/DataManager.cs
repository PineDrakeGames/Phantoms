using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    private const string DATA_MANAGER_PREFAB = "Assets/Resources/Data Manager";

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
        get { return s_instance.m_phantomData; }
    }


    public void Initialize()
    {
        // Any initialization things
        DontDestroyOnLoad(this.gameObject);
    }
}
