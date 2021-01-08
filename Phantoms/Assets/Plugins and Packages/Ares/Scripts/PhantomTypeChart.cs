using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AttackType
{
    public TypeMultiplier[] TypeMatchups = new TypeMultiplier[System.Enum.GetValues(typeof(PhantomType)).Length];
}

[CreateAssetMenu(menuName = "PhantomTypeChart")]
public class PhantomTypeChart : ScriptableObject, ISerializationCallbackReceiver
{

    [SerializeField]
    private AttackType[] m_attackTypes = new AttackType[System.Enum.GetValues(typeof(PhantomType)).Length];

    public Dictionary<PhantomType, Dictionary<PhantomType, TypeMultiplier>> TypeChart = new Dictionary<PhantomType, Dictionary<PhantomType, TypeMultiplier>>();

    public void OnBeforeSerialize()
    {
        PhantomType[] types = System.Enum.GetValues(typeof(PhantomType)) as PhantomType[];
        m_attackTypes = new AttackType[types.Length];
        for (int i = 0; i < types.Length; i++)
        {
            AttackType type = new AttackType();
            TypeMultiplier[] multiplier = new TypeMultiplier[types.Length];

            for (int j = 0; j < types.Length; j++)
            {
                multiplier[j] = TypeChart[types[i]][types[j]];
            }
            type.TypeMatchups = multiplier;
            m_attackTypes[i] = type;
        }
    }

    public void OnAfterDeserialize()
    {
        TypeChart = new Dictionary<PhantomType, Dictionary<PhantomType, TypeMultiplier>>();

        PhantomType[] types = System.Enum.GetValues(typeof(PhantomType)) as PhantomType[];
        for (int i = 0; i < types.Length; i++)
        {
            Dictionary<PhantomType, TypeMultiplier> typeMatchups = new Dictionary<PhantomType, TypeMultiplier>();

            for (int j = 0; j < types.Length; j++)
            {
                typeMatchups[types[j]] = m_attackTypes[i].TypeMatchups[j];
            }
            TypeChart[types[i]] = typeMatchups;
        }
    }
}