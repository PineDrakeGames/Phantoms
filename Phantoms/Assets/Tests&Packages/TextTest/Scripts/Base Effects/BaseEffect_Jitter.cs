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
    [Tooltip("The pivot point for the rotation.")]
    private float m_minJitterDistance = 0f;

    [SerializeField]
    [Tooltip("The time in seconds before the text is jittered again.")]
    private float m_timeBetweenJitter = .033333f; // Default value is 1/30th of a second, jittering at 30fps.

    [HideInInspector]
    [SerializeField]
    private Vector2[] m_randomPositions = null;

    public override void OnValidate()
    {
        base.OnValidate();

        if (m_minJitterDistance <= 0f)
        {
            m_minJitterDistance = 0f;
        }
        if (m_maxJitterDistance < m_minJitterDistance)
        {
            m_maxJitterDistance = m_minJitterDistance;
        }

        if (m_timeBetweenJitter <= 0f)
        {
            m_timeBetweenJitter = 0.001f;
        }
        if (m_timeBetweenJitter > m_effectPeriod)
        {
            m_effectPeriod = m_timeBetweenJitter;
        }

        SetPositions();
    }

    protected override void ApplyEffect(float progress, CharacterData data)
    {
        float size = data.GetCharacterDimensionsSource().magnitude;

        int index = Mathf.FloorToInt(progress * (float)m_randomPositions.Length);

        float randx = m_randomPositions[index].x * size;
        float randy = m_randomPositions[index].y * size;

        int[] vertices = m_vertices.Vertices();
        for (int i = 0; i < vertices.Length; i++)
        {
            int vert = vertices[i];
            Vector3 newPosition = data.GetVertexPositionCurrent(vert);
            newPosition.x += randx;
            newPosition.y += randx;
            data.SetVertexPosition(vert, newPosition);
        }
    }

    private void SetPositions()
    {
        int numJitters = Mathf.FloorToInt(m_effectPeriod / m_timeBetweenJitter);
        m_randomPositions = new Vector2[numJitters];
        for (int i = 0; i < numJitters; i++)
        {
            float x = (Random.Range(0f, 2f) * 2f - 1f) * Random.Range(m_minJitterDistance, m_maxJitterDistance);
            float y = (Random.Range(0f, 2f) * 2f - 1f) * Random.Range(m_minJitterDistance, m_maxJitterDistance);
            m_randomPositions[i] = new Vector2(x, y);
        }
    }
}
