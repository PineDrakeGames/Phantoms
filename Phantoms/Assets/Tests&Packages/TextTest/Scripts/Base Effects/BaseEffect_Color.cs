using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Color", menuName = "FancyText/BaseEffect/Color", order = 2)]
public class BaseEffect_Color : BaseEffect
{
    private enum ColorBlendMode
    {
        SET,        // Simply sets the color to the value
        MULTIPLY,   // Multiplies the two colors together.
        ADD         // Adds the two colors together.
    }

    [SerializeField]
    [Tooltip("Chooses the method of blending colors.")]
    private ColorBlendMode m_colorBlendMode = ColorBlendMode.SET;

    [SerializeField]
    [Tooltip("Chooses how much to blend the colors - with 0 just being the previous color, and 1 being the new color.")]
    [Range(0f, 1f)]
    private float m_colorBlendStrength = 1f;

    [SerializeField]
    [Tooltip("The color gradient to animate the text over")]
    private Gradient m_colorValue = null;

    protected override void ApplyEffect(float progress, int vertexIndex, Vector3[] sourceVertices, ref Vector3[] destinationVertices, ref Color32[] newVertexColors)
    {
        Color color = m_colorValue.Evaluate(progress);

        int[] vertices = m_vertices.Vertices();
        for (int i = 0; i < vertices.Length; i++)
        {
            int vert = vertices[i];

            Color defaultColor = newVertexColors[vertexIndex + vert];

            Color targetColor = color;

            switch (m_colorBlendMode)
            {
                case ColorBlendMode.MULTIPLY:
                    targetColor = defaultColor * color;
                    break;
                case ColorBlendMode.ADD:
                    targetColor = defaultColor + color;
                    break;
                case ColorBlendMode.SET:
                    // Do nothing, leave target color as default;
                    break;
            }

            newVertexColors[vertexIndex + vert] = Color32.Lerp(defaultColor, targetColor, m_colorBlendStrength);
        }
    }
}
