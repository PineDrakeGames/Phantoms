using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ares;

public class TestBattleStart : MonoBehaviour
{
    [SerializeField]
    private BattleManager m_battleManager = null;

    [SerializeField]
    private Actor[] m_phantomActors = null;

    [SerializeField]
    private PhantomInstanceData[] m_phantomData = null;
    // Start is called before the first frame update
    void Start()
    {
        int Count = Mathf.Min(m_phantomActors.Length, m_phantomData.Length);

        for (int i = 0; i < Count; i++)
        {
            m_phantomData[i].SetCurrentStats();
            PhantomDataUtility.SetPhantomActor(m_phantomActors[i], m_phantomData[i]);
        }

        m_battleManager.StartBattle();
    }
}
