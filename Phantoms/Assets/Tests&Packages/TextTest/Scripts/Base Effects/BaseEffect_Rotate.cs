using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Rotate", menuName = "FancyText/BaseEffect/Rotate", order = 2)]
public class BaseEffect_Rotate : BaseEffect
{
    [Header("Rotation specific variables")]
    [SerializeField]
    [Tooltip("Determines whether to use the height of the character, word, or line when moving the position, to keep movement consistent between neighboring characters.")]
    private CharacterHeight m_characterHeight = CharacterHeight.LINE;

    [SerializeField]
    [Tooltip("The pivot point for the rotation.")]
    private Vector2 m_centerOfRotation = new Vector2(0.5f, 0.5f);

    [SerializeField]
    [Tooltip("The axis of rotation used when rotating the vertices.")]
    private Vector3 m_axisOfRotation = new Vector3(0f, 0f, 1f);

    [SerializeField]
    [Tooltip("A value of 0 is no rotation, with -1 and 1 being completely flipped.")]
    private AnimationCurve m_rotationValue = AnimationCurve.Constant(0f, 1f, 0f);

    protected override void ApplyEffect(float progress, CharacterData data)
    {
        Vector3 center = data.GetPivotSource(m_centerOfRotation, m_characterHeight);

        float rotation = m_rotationValue.Evaluate(progress) * 360f;

        int[] vertices = m_vertices.Vertices();
        for (int i = 0; i < vertices.Length; i++)
        {
            int vert = vertices[i];
            Vector3 dir = data.GetVertexPositionCurrent(vert) - center;
            Vector3 targetDir = Quaternion.Euler(m_axisOfRotation.normalized * rotation) * dir;
            data.SetVertexPosition(vert, (center + targetDir));
        }
    }
}
