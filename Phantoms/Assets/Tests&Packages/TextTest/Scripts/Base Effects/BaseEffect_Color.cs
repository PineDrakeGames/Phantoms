using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Color", menuName = "FancyText/BaseEffect/Color", order = 2)]
public class BaseEffect_Color : BaseEffect
{
    [SerializeField]
    [Tooltip("Chooses how much to blend the colors - with 0 just being the previous color, and 1 being the new color.")]
    [Range(0f, 1f)]
    private float m_colorBlend = 1f;

    [SerializeField]
    [Tooltip("The color gradient to animate the text over")]
    private Gradient m_colorValue = null;

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
