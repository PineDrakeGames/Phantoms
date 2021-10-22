using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    [SerializeField]
    [ConversationPopup]
    private string m_starterQuizConversation = null;
    [SerializeField]
    [VariablePopup]
    private string m_starterQuestionVariable = null;
    [SerializeField]
    [VariablePopup]
    private string m_finalQuestionVariable = null;
    [SerializeField]
    private List<StarterOption> m_starterOptions = new List<StarterOption>();

    [Header("Scene References")]
    [SerializeField]
    private Transform m_playerStartPosition = null;
    [SerializeField]
    private GameObject m_titleScreenStuff;
    [SerializeField]
    private Animator m_blackScreenAnimator = null;
    [SerializeField]
    private GameObject m_questionAnswersParent;
    [SerializeField]
    private StarterQuizAnswerButton[] m_answerButtons = null;
    [SerializeField]
    private GameObject m_phantomOptionsParent;
    [SerializeField]
    private StarterQuizPhantomOption[] m_phantomOptionButtons = null;
    [SerializeField]
    private GameObject m_titleScreenCamera = null;

    [SerializeField]
    [Scene]
    private string m_titleScene = null;
    [SerializeField]
    [Scene]
    private string m_startScene = null;

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
    private Dictionary<string, int> m_relatedAnswers = new Dictionary<string, int>(); // Used for tiebreakers
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

    // Called to start the whole quiz! Will set stuff up and start the dialogue.
    public void StartQuiz()
    {
        foreach (StarterOption option in m_starterOptions)
        {
            m_starterScores[option.QuizID.Trim().ToUpper()] = 0;
            m_relatedAnswers[option.QuizID.Trim().ToUpper()] = 0;
        }

        OverworldManager.Instance.PlayerController.transform.position = m_playerStartPosition.position;
        OverworldManager.Instance.PlayerController.transform.rotation = m_playerStartPosition.rotation;

        DialogueManager.StartConversation(m_starterQuizConversation);
    }

    // Called each time we need to get a new question - picks one out and sets the right variables in the dialogue system.
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
        DialogueLua.SetVariable(m_starterQuestionVariable, m_currentQuestion.Question);

        // check if it's the last question
        m_currentQuestionIndex++;
        if (m_currentQuestionIndex == m_questions.Questions.Count)
        {
            DialogueLua.SetVariable(m_finalQuestionVariable, true);
        }
    }

    // Called to display the answer options for the current question as buttons
    public void ShowAnswers()
    {
        if (m_currentQuestion != null)
            m_questionAnswersParent.SetActive(true);

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

    // Called by the answer option buttons
    public void SelectAnswer(StarterQuizQuestions.StarterQuestionAnswer answer)
    {
        foreach (StarterQuizQuestions.StarterQuestionValue value in answer.Values)
        {
            string valueID = value.ID.Trim().ToUpper();
            if (m_starterScores.ContainsKey(valueID))
            {
                m_starterScores[valueID] += value.Value;
                if (value.Value > 0)
                {
                    m_relatedAnswers[valueID] += 1;
                }
            }
            else
            {
                m_starterScores.Add(valueID, value.Value);
                if (value.Value > 0)
                {
                    m_relatedAnswers[valueID] = 1;
                }
            }
        }

        

        m_questionAnswersParent.SetActive(false);
        OnAnswerSelect.Invoke();
    }

    // Called to show the phantom options
    public void ShowPhantoms()
    {
        m_phantomOptionsParent.SetActive(true);

        List<PhantomData> options = GetStarterOptionPhantoms();

        for (int i = 0; i < m_phantomOptionButtons.Length; i++)
        {

            StarterQuizPhantomOption button = m_phantomOptionButtons[i];
            if (i < options.Count)
            {
                button.gameObject.SetActive(true);
                button.Phantom = options[i];
            }
            else
            {
                button.gameObject.SetActive(false);

            }
        }
    }

    public void SelectPhantom(PhantomData phantom)
    {
        // TODO: Generate the instance for the phantom, continue convo, etc.
        OnAnswerSelect.Invoke();
        m_phantomOptionsParent.SetActive(false);

        PhantomInstanceData phantomInstance = PhantomDataUtility.GenerateRandomPhantom(phantom, 1);

        PlayerInventoryManager.Instance.AddPhantom(phantomInstance);

        DataManager.Instance.ChosenStarterID = phantom.ID;
    }

    // Functions for finishing sequence stuff //
    public void TurnOnPlayer()
    {
        Player.PlayerInputEnabled = false;
        OverworldManager.Instance.PlayerInstance.SetActive(true);
        OverworldManager.Instance.PlayerController.SetState(new PlayerStateInteract());
        // For some reason, have to manually simulate a frame before setting the player's position and rotation.
        KinematicCharacterController.KinematicCharacterSystem.Simulate(Time.fixedDeltaTime, KinematicCharacterController.KinematicCharacterSystem.CharacterMotors, KinematicCharacterController.KinematicCharacterSystem.PhysicsMovers);
        OverworldManager.Instance.PlayerController.Motor.SetPositionAndRotation(m_playerStartPosition.position, m_playerStartPosition.rotation);
        LoadingManager.NewSceneLoaded.Invoke(); // Done to re-update shaders
    }

    public void SetBlackScreen(bool visible)
    {
        m_blackScreenAnimator.SetBool("Visible", visible);
    }

    public void FinishQuiz()
    {
        StartCoroutine(EndQuiz());
    }


    ////////////////////////////////
    /// Private Helper Functions ///
    ////////////////////////////////

    // Gets the list of phantoms that the player will choose from for their starter
    private List<PhantomData> GetStarterOptionPhantoms()
    {
        List<string> starterIDs = new List<string>(m_starterScores.Keys);
        starterIDs.Sort((x, y) => (m_starterScores[y].CompareTo(m_starterScores[x]))); // Sort all the starter scores based on score!

        // If the 3rd and 4th are tied, sort by the number of answers related to that specific starter instead of just points.
        if (m_starterScores[starterIDs[2]] == m_starterScores[starterIDs[3]])
        {
            // Get all the starter id's with that value, then sort.
            List<string> tiedValues = new List<string>();
            int tiedScore = m_starterScores[starterIDs[2]];
            int numTied = 0;
            foreach (string starterID in m_starterScores.Keys)
            {
                if (m_starterScores[starterID] == tiedScore)
                {
                    tiedValues.Add(starterID);
                    for (int i = 0; i < 3; i++)
                    {
                        if (starterID == starterIDs[i])
                        {
                            numTied++;
                        }
                    }
                }
            }

            tiedValues.Sort((x, y) => (m_relatedAnswers[y].CompareTo(m_relatedAnswers[x])));

            // If things are still tied, time to pick randomly!
            if (m_relatedAnswers[tiedValues[numTied - 1]] == m_relatedAnswers[tiedValues[numTied]])
            {
                // Can be smarter, but honestly this is such a low chance already I'm just shufflin everything.
                tiedValues.Shuffle<string>();
            }

            for (int i = 0; i < numTied; i++)
            {
                starterIDs[2-i] =  tiedValues[i];
            }
        }

        for (int i = 0; i < starterIDs.Count; i++)
        {
            Debug.Log((i+1) + ": " + starterIDs[i] + " (" + m_starterScores[starterIDs[i]] + "," + m_relatedAnswers[starterIDs[i]] + ")");
        }

        List<PhantomData> result = new List<PhantomData>();

        for (int i = 0; i < 3; i++)
        {
            foreach(StarterOption option in m_starterOptions)
            {
                if (option.QuizID == starterIDs[i])
                {
                    result.Add(option.Phantom);
                    DataManager.Instance.StarterChoices.Add(option.Phantom.ID);
                    break;
                }
            }
        }

        // Saving the order of starter results, so we know what the first 3 options are as well as the lowest options, because hey why not maybe we can use that later
        for (int i = 0; i < starterIDs.Count; i++)
        {
            foreach(StarterOption option in m_starterOptions)
            {
                if (option.QuizID == starterIDs[i])
                {
                    DataManager.Instance.StartersOrder.Add(option.Phantom.ID);
                    break;
                }
            }
        }

        return result;
    }

    private IEnumerator EndQuiz()
    {
        Scene titleScene = SceneManager.GetSceneByPath(m_titleScene);
        Scene startScene = SceneManager.GetSceneByPath(m_startScene);

        SceneManager.SetActiveScene(startScene);
        OverworldManager.Instance.SetOverworldActive(true);
        Player.PlayerInputEnabled = true;
        OverworldManager.Instance.PlayerController.SetState(new PlayerStateIdle());


        yield return null;
        if (m_titleScreenCamera != null)
        {
            Destroy(m_titleScreenCamera);
        }
        yield return new WaitForSeconds(5f);
        yield return SceneManager.UnloadSceneAsync(m_titleScene);
    }

}
