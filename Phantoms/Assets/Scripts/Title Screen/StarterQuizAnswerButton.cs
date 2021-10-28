using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class StarterQuizAnswerButton : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField]
    TMP_Text m_buttonText = null;

    [Header("Scene References")]
    [SerializeField]
    StarterQuizManager m_manager = null;

    private Button m_buttonComponent = null;
    public Button ButtonComponent { get { return m_buttonComponent; } }

    private StarterQuizQuestions.StarterQuestionAnswer m_answer = null;
    public StarterQuizQuestions.StarterQuestionAnswer Answer
    {
        get { return m_answer; }
        set { SetAnswer(value); }
    }

    ///////////////////////
    /// Unity Functions ///
    ///////////////////////
    private void Awake()
    {
        m_buttonComponent = GetComponent<Button>();
        m_buttonComponent.onClick.AddListener(OnClick);
    }

    private void OnDestroy() 
    {
        if (m_buttonComponent)
        {
            m_buttonComponent.onClick.RemoveAllListeners();
        }
    }


    ////////////////////////
    /// Public Functions ///
    ////////////////////////
    public void OnClick()
    {
        if (m_manager != null && m_answer != null)
        {
            m_manager.SelectAnswer(m_answer);
        }
    }

    public void SetAnswer(StarterQuizQuestions.StarterQuestionAnswer newAnswer)
    {
        m_answer = newAnswer;
        m_buttonText.text = newAnswer.Answer;
    }
}
