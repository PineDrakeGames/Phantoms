using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Text Effect Table", menuName = "FancyText/FancyTextEffectTable", order = 0)]
public class FancyTextEffectTable : ScriptableObject
{
    [SerializeField]
    [Tooltip("List of all possible text effects")]
    private FancyTextEffect[] m_fancyTextEffects = null;

    // Private dictionary used to map keys to specific fancy text effects for faster lookup.
    private Dictionary<string, FancyTextEffect> m_keyToTextEffect = null;


    /// <summary>
    /// Returns a Fancy text effect for a given key if it exists, otherwise returns null.
    /// </summary>
    /// <param name="textEffectKey">The key for the desired text effect</param>
    /// <returns>Returns a FancyTextEffect scriptable object based on the passed value.</returns>
    public FancyTextEffect GetTextEffect(string textEffectKey)
    {
        if (m_keyToTextEffect == null)
        {
            InitializeTextEffectDictionary();
        }

        if (m_keyToTextEffect.ContainsKey(textEffectKey))
        {
            return m_keyToTextEffect[textEffectKey];
        }
        else
        {
            return null;
        }
    }

    // Private function to initialize the text effect dictionary. Should be called if the dictionary is null.
    private void InitializeTextEffectDictionary()
    {
        m_keyToTextEffect = new Dictionary<string, FancyTextEffect>();
        foreach (FancyTextEffect textEffect in m_fancyTextEffects)
        {
            foreach (string key in textEffect.EffectKeys)
            {
                key.ToLower();
                if (!m_keyToTextEffect.ContainsKey(key))
                {
                    m_keyToTextEffect.Add(key, textEffect);
                }
                /// TODO: Not sure what to do if multiple of the same key are found?
            }
        }
    }
}
