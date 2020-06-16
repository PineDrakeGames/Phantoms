using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Text Effect", menuName = "FancyText/FancyTextEffect", order = 1)]
public class FancyTextEffect : ScriptableObject
{
    [SerializeField]
    [Tooltip("The ID for this given effect - mostly used as a display name.")]
    private string m_effectID = "Default";

    [SerializeField]
    [Tooltip("List of keys that will be recognized as this text effect.")]
    private string[] m_effectKeys = null;

    [SerializeField]
    [Tooltip("Effects that will be applied to a given character, done so in the order assigned to the array.")]
    private BaseEffect[] m_effects;


    public string[] EffectKeys
    {
        get { return m_effectKeys;}
    }

    public void ApplyEffect(int characterIndex, int vertexIndex, Vector3[] sourceVertices, ref Vector3[] destinationVertices, ref Color32[] newVertexColors)
    {
        for(int i = 0; i < m_effects.Length; i++)
        {
            m_effects[i].ApplyEffect(characterIndex, vertexIndex, sourceVertices, ref destinationVertices, ref newVertexColors);
        }
    }
}
