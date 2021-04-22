using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoveMinigameTarget : MonoBehaviour
{
    public LoveMinigame Minigame = null;

    private RectTransform m_targetTransform = null;

    /////////////////////////////////////////
    /// Public Functions to set up target ///
    /////////////////////////////////////////
    public void SetPosition(Vector2 position)
    {
        m_targetTransform.anchoredPosition = position;
    }
    
    public void Hit()
    {
        this.gameObject.SetActive(false);
    }

    ///////////////////////
    /// Unity Functions ///
    ///////////////////////
    private void Awake()
    {
        m_targetTransform = GetComponent<RectTransform>();    
    }

}
