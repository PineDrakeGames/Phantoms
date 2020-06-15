using UnityEngine;

public abstract class BaseEffect : ScriptableObject
{
    [SerializeField][Header("Affected Vertices")]
    protected FancyVertices m_vertices = null;

    public abstract void ApplyEffect(float time, int characterIndex, int vertexIndex, Vector3[] sourceVertices, ref Vector3[] destinationVertices, ref Color32[] newVertexColors);
}