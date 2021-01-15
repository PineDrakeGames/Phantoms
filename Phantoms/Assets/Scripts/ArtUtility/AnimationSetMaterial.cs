using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationSetMaterial : MonoBehaviour
{
    [SerializeField]
    private Renderer m_renderer = null;

    [SerializeField]
    private AwesomeToon.AwesomeToonHelper m_toonHelper = null;

    [SerializeField]
    private int m_materialIndex = 0;

    [SerializeField]
    private Material[] m_materials = null;


    private int m_currentMaterial = 0;
    public int CurrentMaterial
    {
        get { return m_currentMaterial; }
        set
        {
            m_currentMaterial = value;
            UpdateCurrentMaterial();
        }
    }

    public void UpdateCurrentMaterial(int newMaterial)
    {
        CurrentMaterial = newMaterial;
        Debug.Log("Update that shhizzz");
    }

    public void UpdateCurrentMaterial()
    {
        if (m_materials != null && m_renderer != null)
        {
            if ((m_currentMaterial < m_materials.Length) && (m_materialIndex < m_renderer.materials.Length))
            {
                m_renderer.materials[m_materialIndex] = m_materials[m_currentMaterial];
                if (m_toonHelper)
                {
                    m_toonHelper.material = m_materials[m_currentMaterial];
                    m_toonHelper.Init();
                }
            }
        }
    }
}
