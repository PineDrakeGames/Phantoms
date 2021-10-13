using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RelicInstance
{
    public RelicData Data = null;
    public bool Equipped = false;
    private UserBattleInstanceData m_user = null;
    public UserBattleInstanceData User
    {
        get { return m_user; }
        set
        {
            m_user = value;
            if (m_user != null)
            {
                UserID = m_user.InstanceID;
            }
            else
            {
                UserID = "";
            }
        }
    }
    public string UserID = null;

    public RelicInstance()
    {
        Data = null;
        Equipped = false;
        User = null;
    }

    public RelicInstance(RelicData data)
    {
        Data = data;
        Equipped = false;
        User = null;
    }
}
