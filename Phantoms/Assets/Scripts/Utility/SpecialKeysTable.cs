using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpecialKey
{
    NONE,
    WASD,
    SPACEBAR,
    LMB,
    RMB,
    MMB
}

[CreateAssetMenu(menuName = "Phantoms/Special Key Table")]
public class SpecialKeysTable : ScriptableObject, ISerializationCallbackReceiver
{
    [SerializeField]
    [EnumNamedArray(typeof(SpecialKey))]
    private Sprite[] m_specialKeyIcon = new Sprite[System.Enum.GetValues(typeof(SpecialKey)).Length];

    public Dictionary<SpecialKey, Sprite> KeyToIcon = new Dictionary<SpecialKey, Sprite>();

    public void OnBeforeSerialize()
    {

        SpecialKey[] specialKeys = System.Enum.GetValues(typeof(SpecialKey)) as SpecialKey[];
        m_specialKeyIcon = new Sprite[specialKeys.Length];

        for (int i = 0; i < specialKeys.Length; i++)
        {
            if (KeyToIcon.ContainsKey(specialKeys[i]))
            {
                m_specialKeyIcon[i] = KeyToIcon[specialKeys[i]];
            }
        }
    }

    public void OnAfterDeserialize()
    {
        SpecialKey[] specialKeys = System.Enum.GetValues(typeof(SpecialKey)) as SpecialKey[];
        KeyToIcon.Clear();

        for (int i = 0; i < specialKeys.Length; i++)
        {
            KeyToIcon[specialKeys[i]] = m_specialKeyIcon[i];
        }
    }
}
