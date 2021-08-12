using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarterQuizManager : MonoBehaviour
{
    [System.Serializable]
    private class StarterOption
    {
        public string QuizID = "ANGER";
        public PhantomData Phantom = null;
    }

    [SerializeField]
    private List<StarterOption> m_starterOptions = new List<StarterOption>();


    private Dictionary<string, int> m_starterScores = new Dictionary<string, int>();


    public void StartQuiz()
    {
        foreach(StarterOption option in m_starterOptions)
        {
            m_starterScores[option.QuizID] = 0;
        }
    }
}
