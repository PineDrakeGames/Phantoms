using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using PixelCrushers.DialogueSystem;

public class StarterQuizManager : MonoBehaviour
{
    [System.Serializable]
    private class StarterOption
    {
        public string QuizID = "ANGER";
        public PhantomData Phantom = null;
    }


    [Header("Data References")]
    [SerializeField]
    private StarterQuizQuestions m_questions = null;
    [SerializeField] [ConversationPopup]
    private string m_starterQuizConversation = null;
    [SerializeField]
    private List<StarterOption> m_starterOptions = new List<StarterOption>();

    [Header("Scene References")]
    [SerializeField]
    private GameObject m_titleScreenStuff;
    [SerializeField]
    private GameObject m_questionAnswersParent;
    [SerializeField]
    private StarterQuizAnswerButton[] m_answerButtons = null;

    [Header("Events")]
    public UnityEvent OnAnswerSelect = new UnityEvent();

    // static instance
    private static StarterQuizManager s_instance = null;
    public static StarterQuizManager Instance
    {
        get
        {  
            if (s_instance == null)
            {
                s_instance = FindObjectOfType<StarterQuizManager>();
            }
            return s_instance;
        }
    }

    // private variables
    private Dictionary<string, int> m_starterScores = new Dictionary<string, int>();
    private int m_currentQuestionIndex = 0;
    private StarterQuizQuestions.StarterQuestion m_currentQuestion = null;


    ///////////////////////
    /// Unity Functions ///
    ///////////////////////
    private void Awake() 
    {
        if (s_instance == null) { s_instance = this; }
        else if (s_instance != this) { Destroy(this); }
    }


    /////////////////////////////
    /// Public Quiz Functions ///
    /////////////////////////////
    public void StartQuiz()
    {
        foreach(StarterOption option in m_starterOptions)
        {
            m_starterScores[option.QuizID] = 0;
        }

        m_titleScreenStuff.SetActive(false);

        DialogueManager.StartConversation(m_starterQuizConversation);
    }

    public void GenerateQuestion()
    {
        // Make sure we have a question
        if (m_currentQuestionIndex >= m_questions.Questions.Count)
        {
            return;
        }

        // For now, just going down the list.
        m_currentQuestion = m_questions.Questions[m_currentQuestionIndex];

        // Once we get the current question, update it in the dialogue system
        DialogueLua.SetVariable("Intro.Starter Question", m_currentQuestion.Question);

        // check if it's the last question
        m_currentQuestionIndex++;
        if (m_currentQuestionIndex == m_questions.Questions.Count)
        {
            DialogueLua.SetVariable("Intro.Last Question", true);
        }
    }

    public void ShowAnswers()
    {
        if (m_currentQuestion != null)
        m_questionAnswersParent.SetActive(true);

        Debug.Log(m_currentQuestion.Answers.Count);

        for (int i = 0; i < m_answerButtons.Length; i++)
        {

            StarterQuizAnswerButton button = m_answerButtons[i];
            if (i < m_currentQuestion.Answers.Count)
            {
                button.gameObject.SetActive(true);
                button.Answer = m_currentQuestion.Answers[i];
            }
            else
            {
                button.gameObject.SetActive(false);
                
            }
        }
    }

    public void SelectAnswer(StarterQuizQuestions.StarterQuestionAnswer answer)
    {

        m_questionAnswersParent.SetActive(false);
        OnAnswerSelect.Invoke();
    }
}
