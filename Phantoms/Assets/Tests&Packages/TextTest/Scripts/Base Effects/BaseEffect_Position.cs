using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Position", menuName = "FancyText/BaseEffect/Position", order = 2)]
public class BaseEffect_Position : BaseEffect
{
    [SerializeField]
    [Tooltip("The time in seconds it takes to scrub through the animation curve of this effect.")]
    private float m_effectPeriod = 1f;

    [SerializeField]
    [Tooltip("The time in seconds it takes to scrub through the animation curve of this effect.")]
    private float m_characterDelay = 0.2f;

    [SerializeField]
    [Tooltip("A value of 0 is base, with -1 and 1 being the width of the character")]
    private AnimationCurve m_xPosition = AnimationCurve.Constant(0f, 1f, 0f);

    [SerializeField]
    [Tooltip("A value of 0 is base, with -1 and 1 being the height of the character")]
    private AnimationCurve m_yPosition = AnimationCurve.Constant(0f, 1f, 0f);

    public override void ApplyEffect(float time, int characterIndex, int vertexIndex, Vector3[] sourceVertices, ref Vector3[] destinationVertices, ref Color32[] newVertexColors)
    {
        float characterTime = Mathf.Abs((time - (m_characterDelay * characterIndex)) % m_effectPeriod);

        Vector3 dimensions = (sourceVertices[vertexIndex + 0] - sourceVertices[vertexIndex + 2]);

        Vector3 offset = Vector3.Scale(dimensions, new Vector3(m_xPosition.Evaluate(characterTime / m_effectPeriod), m_yPosition.Evaluate(characterTime / m_effectPeriod), 0f));

        for (int i = 0; i < 4; i++)
        {
            destinationVertices[vertexIndex + i] = sourceVertices[vertexIndex + i] + offset;
        }
    }
}
