using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class FancyVertices
{

    [SerializeField]
    private bool[] m_vertices = new bool[4] { true, true, true, true };

    private List<int> selectedVertices;

    public int[] Vertices()
    {
        selectedVertices = new List<int>();
        for (int i = 0; i < m_vertices.Length; i++)
        { 
            if (m_vertices[i])
            {
                selectedVertices.Add(i);
            }
        }

        return selectedVertices.ToArray();
    }
}
