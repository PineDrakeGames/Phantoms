using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharacterData
{
    // Various variables referencing things in the text info directly - so we can get initial references to any data we need and hold the references.
    private TMP_TextInfo m_textInfo;
    private int m_characterIndex;
    private int m_materialIndex;
    private int m_vertexIndex;
    private TMP_CharacterInfo m_characterInfo;
    private TMP_MeshInfo m_cachedMeshInfo;
    private TMP_MeshInfo m_meshInfo;
    private Vector3[] m_sourceVertices = null;
    private Vector3[] m_destinationVertices = null;
    private Color32[] m_sourceVertexColors;
    private Color32[] m_vertexColors;

    /// Constructor and method to update character data ///
    public CharacterData(int characterIndex, TMP_TextInfo textInfo)
    {
        UpdateData(characterIndex, textInfo);
    }

    public void UpdateData(int characterIndex, TMP_TextInfo textInfo)
    {
        // First, set the text info and character index - which are needed to get the rest of the data.
        m_textInfo = textInfo;
        m_characterIndex = characterIndex;

        UpdateData();
    }

    public void UpdateData()
    {
        // Ensure that the character is actually visible - no need to do anything else if this is not the case.
        m_characterInfo = m_textInfo.characterInfo[m_characterIndex];
        if (!m_characterInfo.isVisible)
        {
            return;
        }

        // Get any indexes for specific data in the text info arrays.
        m_materialIndex = m_characterInfo.materialReferenceIndex;
        m_vertexIndex = m_characterInfo.vertexIndex;

        // Get references to more specific pieces of info within the text info.
        m_cachedMeshInfo = m_textInfo.CopyMeshInfoVertexData()[m_materialIndex];
        m_meshInfo = m_textInfo.meshInfo[m_materialIndex];

        // Get references to data from the cached mesh.
        m_sourceVertices = m_cachedMeshInfo.vertices;
        m_sourceVertexColors = m_cachedMeshInfo.colors32;

        // Get references to data from the current mesh.
        m_destinationVertices = m_meshInfo.vertices;
        m_vertexColors = m_meshInfo.colors32;
    }


    /////////////////////////////
    /// Data Getter Functions ///
    /////////////////////////////


    /// Variable data ///
    public int CharacterIndex
    {
        get { return m_characterIndex; }
    }

    public bool IsVisible
    {
        get { return m_characterInfo.isVisible; }
    }


    /// Data about the whole character ///
    public Vector3 GetCharacterDimensionsSource()
    {
        return (m_sourceVertices[m_vertexIndex + 0] - m_sourceVertices[m_vertexIndex + 2]);
    }

    public Vector3 GetCharacterDimensionsCurrent()
    {
        return (m_destinationVertices[m_vertexIndex + 0] - m_destinationVertices[m_vertexIndex + 2]);
    }

    public Vector3 GetPivotSource(Vector2 pivotPoint)
    {
        Vector3 pointA = Vector3.Lerp(m_sourceVertices[m_vertexIndex + 0], m_sourceVertices[m_vertexIndex + 3], pivotPoint.x);
        Vector3 pointB = Vector3.Lerp(m_sourceVertices[m_vertexIndex + 1], m_sourceVertices[m_vertexIndex + 2], pivotPoint.x);

        return Vector3.Lerp(pointA, pointB, pivotPoint.y);
    }

    public Vector3 GetPivotCurrent(Vector2 pivotPoint)
    {
        Vector3 pointA = Vector3.Lerp(m_destinationVertices[m_vertexIndex + 0], m_destinationVertices[m_vertexIndex + 3], pivotPoint.x);
        Vector3 pointB = Vector3.Lerp(m_destinationVertices[m_vertexIndex + 1], m_destinationVertices[m_vertexIndex + 2], pivotPoint.x);

        return Vector3.Lerp(pointA, pointB, pivotPoint.y);
    }


    /// Data about specific vertices ///
    public Vector3 GetVertexPositionSource(int vertice)
    {
        return m_sourceVertices[m_vertexIndex + vertice];
    }

    public Vector3 GetVertexPositionCurrent(int vertice)
    {
        return m_destinationVertices[m_vertexIndex + vertice];
    }

    public Color32 GetVertexColorSource(int vertice)
    {
        return m_sourceVertexColors[m_vertexIndex + vertice];
    }

    public Color32 GetVertexColorCurrent(int vertice)
    {
        return m_vertexColors[m_vertexIndex + vertice];
    }



    /////////////////////////////
    /// Data Setter Functions ///
    /////////////////////////////
    public void SetVertexPosition(int vertice, Vector3 newPosition)
    {
        m_destinationVertices[m_vertexIndex + vertice] = newPosition;
    }

    public void SetVertexColor(int vertice, Color32 newColor)
    {
        m_vertexColors[m_vertexIndex + vertice] = newColor;
    }
}
