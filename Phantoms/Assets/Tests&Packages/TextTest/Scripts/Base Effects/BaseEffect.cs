using UnityEngine;

public abstract class BaseEffect : ScriptableObject
{
    public abstract void ApplyEffect(float time, int characterIndex, int vertexIndex, Vector3[] sourceVertices, ref Vector3[] destinationVertices, ref Color32[] newVertexColors);
}