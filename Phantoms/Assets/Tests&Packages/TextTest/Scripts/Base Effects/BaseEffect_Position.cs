using UnityEngine;

[CreateAssetMenu(fileName = "Position", menuName = "FancyText/BaseEffect/Position", order = 2)]
public class BaseEffect_Position : BaseEffect
{
    [Header("Position-specific variables")]
    [SerializeField]
    [Tooltip("Determines whether to use the height of the character, word, or line when moving the position, to keep movement consistent between neighboring characters.")]
    private CharacterHeight m_characterHeight = CharacterHeight.LINE;

    [SerializeField]
    [Tooltip("A value of 0 is base, with -1 and 1 being the width of the character")]
    private AnimationCurve m_xPosition = AnimationCurve.Constant(0f, 1f, 0f);

    [SerializeField]
    [Tooltip("A value of 0 is base, with -1 and 1 being the height of the character")]
    private AnimationCurve m_yPosition = AnimationCurve.Constant(0f, 1f, 0f);

    protected override void ApplyEffect(float progress, CharacterData data)
    {
        Vector3 dimensions = data.GetCharacterDimensionsSource(m_characterHeight);;
        Vector3 offset = Vector3.Scale(dimensions, new Vector3(m_xPosition.Evaluate(progress), m_yPosition.Evaluate(progress), 0f));

        int[] vertices = m_vertices.Vertices();
        for (int i = 0; i < vertices.Length; i++)
        {
            int vert = vertices[i];
            Vector3 newPosition = data.GetVertexPositionCurrent(vert) + offset;
            data.SetVertexPosition(vert, newPosition);
        }
    }
}
