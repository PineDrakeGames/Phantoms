using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Jitter", menuName = "FancyText/BaseEffect/Jitter", order = 2)]
public class BaseEffect_Jitter : BaseEffect
{

    [SerializeField]
    [Tooltip("The pivot point for the rotation.")]
    private float m_maxJitterDistance = 0.05f;

    [SerializeField]
    [Tooltip("The time in seconds before the text is jittered again.")]
    private float m_timeBetweenJitter = .033333f; // Default value is 1/30th of a second, jittering at 30fps.

    public override void ApplyEffect(float time, int characterIndex, int vertexIndex, Vector3[] sourceVertices, ref Vector3[] destinationVertices, ref Color32[] newVertexColors)
    {
        float size = (sourceVertices[vertexIndex + 0] - sourceVertices[vertexIndex + 2]).magnitude;
        size *= m_maxJitterDistance;
        float randx = Random.Range(-1, 1) * size;
        float randy = Random.Range(-1, 1) * size;

        int[] vertices = m_vertices.Vertices();
        for (int i = 0; i < vertices.Length; i++)
        {
            int vert = vertices[i];
            destinationVertices[vertexIndex + vert].x += randx;
            destinationVertices[vertexIndex + vert].y += randy;
        }
    }
}
