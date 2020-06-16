using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Color", menuName = "FancyText/BaseEffect/Color", order = 2)]
public class BaseEffect_Color : BaseEffect
{
    [SerializeField]
    [Tooltip("The time in seconds it takes to scrub through the Color gradient of this effect.")]
    private float m_effectPeriod = 1f;

    [SerializeField]
    [Tooltip("The time in seconds to delay the effect of this character based on it's index position.")]
    private float m_characterDelay = 0.2f;

    [SerializeField]
    [Tooltip("Chooses how much to blend the colors - with 0 just being the previous color, and 1 being the new color.")]
    [Range(0f, 1f)]
    private float m_colorBlend = 1f;

    [SerializeField]
    [Tooltip("The color gradient to animate the text over")]
    private Gradient m_colorValue = null;

    // Ensure that values are within a proper range.
    public void OnValidate()
    {
        // The effect period cannot be negative or 0. If you want the effect to play backwards, just flip the curves!
        if (m_effectPeriod <= 0)
        {
            m_effectPeriod = 0.01f;
        }
    }

    public override void ApplyEffect(float time, int characterIndex, int vertexIndex, Vector3[] sourceVertices, ref Vector3[] destinationVertices, ref Color32[] newVertexColors)
    {
        float characterTime = Mathf.Abs((time - (m_characterDelay * characterIndex)) % m_effectPeriod);
        Color color = m_colorValue.Evaluate(characterTime / m_effectPeriod);

        Color32 displayColor = color;

        int[] vertices = m_vertices.Vertices();
        for (int i = 0; i < vertices.Length; i++)
        {
            int vert = vertices[i];
            newVertexColors[vertexIndex + vert] = Color32.Lerp(newVertexColors[vertexIndex + vert], displayColor, m_colorBlend);
        }
    }
}
