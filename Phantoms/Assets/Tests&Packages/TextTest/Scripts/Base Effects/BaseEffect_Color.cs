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
    [Tooltip("The color gradient to animate the text over")]
    private Gradient m_colorValue = null;

    public override void ApplyEffect(float time, int characterIndex, int vertexIndex, Vector3[] sourceVertices, ref Vector3[] destinationVertices, ref Color32[] newVertexColors)
    {
        float characterTime = Mathf.Abs((time - (m_characterDelay * characterIndex)) % m_effectPeriod);
        Color color = m_colorValue.Evaluate(characterTime / m_effectPeriod);

        Color32 displayColor = color;

        for (int i = 0; i < 4; i++)
        {
            newVertexColors[vertexIndex + i] = displayColor;
        }
    }
}
