using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Phantoms/Starter Questions")]
public class StarterQuizQuestions : ScriptableObject
{
    [System.Serializable]
    public class StarterQuestionValue
    {
        public string ID = null;
        public int Value = 0;
    }

    [System.Serializable]
    public class StarterQuestionAnswer
    {
        public string Answer = null;
        public List<StarterQuestionValue> Values = new List<StarterQuestionValue>();
    }

    [System.Serializable]
    public class StarterQuestion
    {
        public string Question = null;
        public List<StarterQuestionAnswer> Answers = new List<StarterQuestionAnswer>();
    }

    public List<StarterQuestion> Questions = new List<StarterQuestion>();
}
