using UnityEngine;

public abstract class BaseEffect : ScriptableObject
{
    [Header("Base Effect Variables")]
    [SerializeField]
    protected FancyVertices m_vertices = null;

    [SerializeField]
    [Tooltip("The time in seconds it takes to scrub through the Color gradient of this effect.")]
    protected float m_effectPeriod = 1f;

    [SerializeField]
    [Tooltip("The time in seconds it takes to scrub through the Color gradient of this effect.")]
    protected float m_characterDelay = 0.2f;

    // Ensure that values are within a proper range.
    public virtual void OnValidate()
    {
        // The effect period cannot be negative or 0. If you want the effect to play backwards, just flip the curves!
        if (m_effectPeriod <= 0)
        {
            m_effectPeriod = 0.01f;
        }
    }

    public abstract void ApplyEffect(int characterIndex, int vertexIndex, Vector3[] sourceVertices, ref Vector3[] destinationVertices, ref Color32[] newVertexColors);

    protected float GetProgress(int characterIndex)
    {
        float characterTime = Mathf.Abs((Time.time - (m_characterDelay * characterIndex)) % m_effectPeriod);
        return (characterTime / m_effectPeriod);
    }
}