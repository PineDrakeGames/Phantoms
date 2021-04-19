using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(RectTransform))]
public class UISetColliderSize : MonoBehaviour
{
    private RectTransform m_rectTransform = null;
    private BoxCollider m_collider = null;

    // Start is called before the first frame update
    void Awake()
    {
        m_rectTransform = GetComponent<RectTransform>();
        m_collider = GetComponent<BoxCollider>();
        SetColliderSize();
    }


    private void OnRectTransformDimensionsChange()
    {
        SetColliderSize();
    }

    private void SetColliderSize()
    {
        if (m_collider != null && m_rectTransform != null)
        {
            m_collider.size = new Vector3(m_rectTransform.rect.width, m_rectTransform.rect.height, m_collider.size.z);
        }
    }
}
