using System;
using UnityEngine;
using TMPro;

public enum CharacterHeight
{
    CHARACTER,
    WORD,
    LINE
}

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

    private TMP_WordInfo m_wordInfo;
    private int m_wordIndex;
    private TMP_LineInfo m_lineInfo;
    private int m_lineIndex;


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

        foreach(TMP_WordInfo wordInfo in m_textInfo.wordInfo)
        {
            if (m_characterIndex >= wordInfo.firstCharacterIndex && m_characterIndex <= wordInfo.lastCharacterIndex)
            {
                m_wordInfo = wordInfo;
            }
        }
        float maxHeight = 0;
        for (int i = m_wordInfo.firstCharacterIndex; i <= m_wordInfo.lastCharacterIndex; i++)
        {
            float height = m_textInfo.characterInfo[i].topLeft.y - m_textInfo.characterInfo[i].bottomLeft.y;
            if (height > maxHeight)
            {
                m_wordIndex = m_textInfo.characterInfo[i].vertexIndex;
                maxHeight = height;
            }
        }

        maxHeight = 0;
        m_lineInfo = m_textInfo.lineInfo[m_characterInfo.lineNumber];
        for (int i = m_lineInfo.firstVisibleCharacterIndex; i <= m_lineInfo.lastVisibleCharacterIndex; i++)
        {
            float height = m_textInfo.characterInfo[i].topLeft.y - m_textInfo.characterInfo[i].bottomLeft.y;
            if (height > maxHeight)
            {
                m_lineIndex = m_textInfo.characterInfo[i].vertexIndex;
                maxHeight = height;
            }
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
    public Vector3 GetCharacterDimensionsSource(CharacterHeight context = CharacterHeight.CHARACTER)
    {
        return (GetVertexPositionSource(0, context) - GetVertexPositionSource(2, context));
    }

    public Vector3 GetCharacterDimensionsCurrent(CharacterHeight context = CharacterHeight.CHARACTER)
    {
        return (GetVertexPositionCurrent(0, context) - GetVertexPositionCurrent(2, context));
    }

    public Vector3 GetPivotSource(Vector2 pivotPoint, CharacterHeight context = CharacterHeight.CHARACTER)
    {
        Vector3 pointA = Vector3.Lerp(GetVertexPositionSource(0, context), GetVertexPositionSource(3, context), pivotPoint.x);
        Vector3 pointB = Vector3.Lerp(GetVertexPositionSource(1, context), GetVertexPositionSource(2, context), pivotPoint.x);

        return Vector3.Lerp(pointA, pointB, pivotPoint.y);
    }

    public Vector3 GetPivotCurrent(Vector2 pivotPoint, CharacterHeight context = CharacterHeight.CHARACTER)
    {
        Vector3 pointA = Vector3.Lerp(GetVertexPositionCurrent(0, context), GetVertexPositionCurrent(3, context), pivotPoint.x);
        Vector3 pointB = Vector3.Lerp(GetVertexPositionCurrent(1, context), GetVertexPositionCurrent(2, context), pivotPoint.x);

        return Vector3.Lerp(pointA, pointB, pivotPoint.y);
    }


    /// Data about specific vertices ///
    public Vector3 GetVertexPositionSource(int vertice, CharacterHeight context = CharacterHeight.CHARACTER)
    {
        Vector3 result = m_sourceVertices[m_vertexIndex + vertice];
        switch (context)
        {
            case CharacterHeight.CHARACTER:
                break;
            case CharacterHeight.WORD:
                result.y =  m_sourceVertices[m_wordIndex + vertice].y;
                break;
            case CharacterHeight.LINE:
                if (m_lineIndex + vertice < m_sourceVertices.Length)
                {
                    result.y =  m_sourceVertices[m_lineIndex + vertice].y;
                }
                else
                {
                    // TODO: The issue is caused when a second material is used in the same line - will need to cache the source vertices for that instead.
                }
                break;
        }
        return result;
    }

    public Vector3 GetVertexPositionCurrent(int vertice, CharacterHeight context = CharacterHeight.CHARACTER)
    {
        Vector3 result = m_destinationVertices[m_vertexIndex + vertice];
        switch (context)
        {
            case CharacterHeight.CHARACTER:
                break;
            case CharacterHeight.WORD:
                result.y =  m_destinationVertices[m_wordIndex + vertice].y;
                break;
            case CharacterHeight.LINE:
                result.y =  m_destinationVertices[m_lineIndex + vertice].y;
                break;
        }
        return result;
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
        m_meshInfo.vertices[m_vertexIndex + vertice] = newPosition;
    }

    public void SetVertexColor(int vertice, Color32 newColor)
    {
        m_meshInfo.colors32[m_vertexIndex + vertice] = newColor;
    }


    ////////////////////////////////
    /// Private Helper Functions ///
    ////////////////////////////////

}