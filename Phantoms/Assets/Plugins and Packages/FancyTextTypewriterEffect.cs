using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;


[RequireComponent(typeof(FancyText))]
[DisallowMultipleComponent]
public class FancyTextTypewriterEffect : AbstractTypewriterEffect
{

    private FancyText m_fancyText = null;
    private string m_currentString = null;

    // Inherited abstract members
    // TODO: Implement these!
    public override bool isPlaying { get { return (m_fancyText != null && m_fancyText.Revealing); } }


    public override void Awake()
    {
        m_fancyText = GetComponent<FancyText>();
        m_fancyText.TypeWriterEffect = this;
    }

    public override void Start()
    {
        if (m_currentString != null)
        {
            m_fancyText.SetText(m_currentString);
        }
    }

    public override void OnEnable()
    {
        base.OnEnable();
    }

    public override void OnDisable()
    {
        base.OnEnable();
    }

    public override void StartTyping(string text, int fromIndex = 0)
    {
        if (text != null)
        {
            m_currentString = text;
            m_fancyText.SetText(text, fromIndex);
        }
    }

    public override void StopTyping()
    {
        Stop();
    }

    public override void Stop()
    {
        m_fancyText.FinishLine();
    }
}
