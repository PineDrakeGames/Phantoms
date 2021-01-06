using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Phantoms/Phantom Data Table")]
public class PhantomDataTable : ScriptableObject
{
    public PhantomData[] Data = null;

    private Dictionary<string, PhantomData> m_IdToData = null;
    public Dictionary<string, PhantomData> IdToData
    {
        get
        {
            if (m_IdToData == null)
            {
                m_IdToData = new Dictionary<string, PhantomData>();
                foreach(PhantomData data in Data)
                {
                    m_IdToData.Add(data.PhantomID, data);
                }
            }
            return m_IdToData;
        }
    }
}
