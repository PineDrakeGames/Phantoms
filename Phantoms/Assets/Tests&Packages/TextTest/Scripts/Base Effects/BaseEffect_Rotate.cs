using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Rotate", menuName = "FancyText/BaseEffect/Rotate", order = 2)]
public class BaseEffect_Rotate : BaseEffect
{

    [SerializeField]
    [Tooltip("The pivot point for the rotation.")]
    private Vector2 m_centerOfRotation = new Vector2(0.5f, 0.5f);

    [SerializeField]
    [Tooltip("The axis of rotation used when rotating the vertices.")]
    private Vector3 m_axisOfRotation = new Vector3(0f, 0f, 1f);

    [SerializeField]
    [Tooltip("The time in seconds it takes to scrub through the animation curve of this effect.")]
    private float m_effectPeriod = 1f;

    [SerializeField]
    [Tooltip("The time in seconds it takes to scrub through the animation curve of this effect.")]
    private float m_characterDelay = 0.2f;

    [SerializeField]
    [Tooltip("A value of 0 is no rotation, with -1 and 1 being completely flipped.")]
    private AnimationCurve m_rotationValue = AnimationCurve.Constant(0f, 1f, 0f);

    public override void ApplyEffect(float time, int characterIndex, int vertexIndex, Vector3[] sourceVertices, ref Vector3[] destinationVertices, ref Color32[] newVertexColors)
    {
        Vector3 pivot = m_centerOfRotation;
        Vector3 center = sourceVertices[vertexIndex + 2] + Vector3.Scale((sourceVertices[vertexIndex + 0] - sourceVertices[vertexIndex + 2]), pivot);

        float characterTime = (Mathf.Abs(time - (m_characterDelay * characterIndex)) % m_effectPeriod);
        float rotation = m_rotationValue.Evaluate(characterTime / m_effectPeriod) * 360f;

        int[] vertices = m_vertices.Vertices();
        for (int i = 0; i < vertices.Length; i++)
        {
            int vert = vertices[i];
            Vector3 dir = sourceVertices[vertexIndex + vert] - center;
            Vector3 targetDir = Quaternion.Euler(m_axisOfRotation.normalized * rotation) * dir;
            destinationVertices[vertexIndex + vert] = center + targetDir;
        }
    }
}
