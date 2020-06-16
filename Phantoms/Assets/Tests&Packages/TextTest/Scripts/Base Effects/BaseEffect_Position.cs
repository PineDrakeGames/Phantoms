using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Position", menuName = "FancyText/BaseEffect/Position", order = 2)]
public class BaseEffect_Position : BaseEffect
{
    [SerializeField]
    [Tooltip("A value of 0 is base, with -1 and 1 being the width of the character")]
    private AnimationCurve m_xPosition = AnimationCurve.Constant(0f, 1f, 0f);

    [SerializeField]
    [Tooltip("A value of 0 is base, with -1 and 1 being the height of the character")]
    private AnimationCurve m_yPosition = AnimationCurve.Constant(0f, 1f, 0f);

    public override void ApplyEffect(int characterIndex, int vertexIndex, Vector3[] sourceVertices, ref Vector3[] destinationVertices, ref Color32[] newVertexColors)
    {
        Vector3 dimensions = (sourceVertices[vertexIndex + 0] - sourceVertices[vertexIndex + 2]);

        float progress = GetProgress(characterIndex);
        Vector3 offset = Vector3.Scale(dimensions, new Vector3(m_xPosition.Evaluate(progress), m_yPosition.Evaluate(progress), 0f));

        int[] vertices = m_vertices.Vertices();
        for (int i = 0; i < vertices.Length; i++)
        {
            int vert = vertices[i];
            destinationVertices[vertexIndex + vert] = destinationVertices[vertexIndex + vert] + offset;
        }
    }
}
