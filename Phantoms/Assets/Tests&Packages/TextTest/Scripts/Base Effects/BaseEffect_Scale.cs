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
    [Tooltip("A value of 1 is default scale, with 0 being the minimum size and 2 being twice as large.")]
    private AnimationCurve m_scaleValue = AnimationCurve.Constant(0f, 1f, 1f);

    protected override void ApplyEffect(float progress, CharacterData data)
    {
        Vector3 center = data.GetPivotSource(m_pivotPoint);

        float scale = m_scaleValue.Evaluate(progress);

        int[] vertices = m_vertices.Vertices();
        for (int i = 0; i < vertices.Length; i++)
        {
            int vert = vertices[i];
            Vector3 dir = data.GetVertexPositionCurrent(vert) - center;
            Vector3 newPosition = center + (dir * scale);
            data.SetVertexPosition(vert, newPosition);
        }
    }
}
