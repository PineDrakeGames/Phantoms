using UnityEngine;
using TMPro;

public enum TextEffectType
{
    CONSTANT,
    CREATOR
}

[CreateAssetMenu(fileName = "Text Effect", menuName = "FancyText/FancyTextEffect", order = 1)]
public class FancyTextEffect : ScriptableObject
{
    /// Serialized Fields ///
    [SerializeField]
    [Tooltip("List of keys that will be recognized as this text effect.")]
    private string[] m_effectKeys = null;

    [SerializeField]
    [Tooltip("The type of text effect that this is.")]
    private TextEffectType m_effectType = TextEffectType.CONSTANT;

    [SerializeField]
    [Tooltip("Effects that will be applied to a given character, done so in the order assigned to the array.")]
    private BaseEffect[] m_effects = null;

    // This variable is serialized based on the other serialized items, but is not publicly editable.
    [SerializeField]
    [HideInInspector]
    private float m_maxEffectTime = 0f;

    /// Publicly accessible Variables ///
    public string[] EffectKeys
    {
        get { return m_effectKeys; }
    }

    public float MaxEffectTime
    {
        get { return m_maxEffectTime; }
    }

    public TextEffectType EffectType
    {
        get { return m_effectType; }
    }


    /// Data validation when things are edited. ///
    private void OnValidate()
    {
        m_maxEffectTime = 0f;
        for (int i = 0; i < m_effects.Length; i++)
        {
            if (m_effects[i] != null)
            {
                float effectPeriod = m_effects[i].EffectPeriod;
                if (effectPeriod > m_maxEffectTime)
                {
                    m_maxEffectTime = effectPeriod;
                }
            }
        }
    }

    /// Public functions to apply the text effect ///
    public void ApplyEffectConstant(CharacterData data)
    {
        for (int i = 0; i < m_effects.Length; i++)
        {
            m_effects[i].ApplyEffectConstant(data);
        }
    }

    public void ApplyEffectCreator(float time, CharacterData data)
    {
        for (int i = 0; i < m_effects.Length; i++)
        {
            m_effects[i].ApplyEffectCreator(time, data);
        }
    }
}
