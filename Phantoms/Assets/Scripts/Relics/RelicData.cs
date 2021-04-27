using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RelicEquipType
{
    PLAYER_ONLY,
    PHANTOM_ONLY,
    BOTH
}

public abstract class RelicData : ScriptableObject
{
    
    [SerializeField] private string m_relicID = null;
    public string ID { get { return m_relicID; } }

    [Header("Display Info")]
    [SerializeField] private string m_relicName = null;
    public string DisplayName { get { return m_relicName; } }
    [SerializeField] private string m_relicDescription = null;
    public string Description { get { return m_relicDescription; } }
    [SerializeField] private Sprite m_relicIcon = null;
    public Sprite Icon { get { return m_relicIcon; } }
    

    [Header("Battle Data")]
    [SerializeField] private RelicEquipType m_equipType = RelicEquipType.BOTH;
    public RelicEquipType EquipType { get { return m_equipType; } }
    [SerializeField] private int m_pointRequirement = 1;
    public int Points { get { return m_pointRequirement; } }
}
