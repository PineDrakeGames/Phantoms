using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Scale", menuName = "FancyText/BaseEffect/Scale", order = 2)]
public class BaseEffect_Scale : BaseEffect
{

    [SerializeField]
    [Tooltip("The pivot point for the scaling.")]
    private Vector2 m_pivotPoint = new Vector2(0.5f, 0.5f);

    [SerializeField]
    [Tooltip("The time in seconds it takes to scrub through the animation curve of this effect.")]
    private float m_effectPeriod = 1f;

    [SerializeField]
    [Tooltip("The time in seconds it takes to scrub through the animation curve of this effect.")]
    private float m_characterDelay = 0.2f;

    [SerializeField]
    [Tooltip("A value of 1 is default scale, with 0 being the minimum size and 2 being twice as large.")]
    private AnimationCurve m_scaleValue = AnimationCurve.Constant(0f, 1f, 1f);

    public override void ApplyEffect(float time, int characterIndex, int vertexIndex, Vector3[] sourceVertices, ref Vector3[] destinationVertices, ref Color32[] newVertexColors)
    {
        Vector3 center = sourceVertices[vertexIndex + 2] + Vector3.Scale((sourceVertices[vertexIndex + 0] - sourceVertices[vertexIndex + 2]), m_pivotPoint);

        float characterTime = Mathf.Abs((time - (m_characterDelay * characterIndex)) % m_effectPeriod);

        float scale = m_scaleValue.Evaluate(characterTime / m_effectPeriod);

        int[] vertices = m_vertices.Vertices();
        for (int i = 0; i < vertices.Length; i++)
        {
            int vert = vertices[i];
            Vector3 dir = sourceVertices[vertexIndex + vert] - center;
            destinationVertices[vertexIndex + vert] = center + (dir * scale);
        }
    }
}
