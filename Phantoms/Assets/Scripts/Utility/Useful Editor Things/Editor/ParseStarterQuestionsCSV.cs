using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ParseStarterQuestionsCSV : EditorWindow
{
    private enum CSVLines
    {
        QUESTION = 0,
        ANSWER_A = 1,
        VALUES_A = 2,
        ANSWER_B = 3,
        VALUES_B = 4,
        ANSWER_C = 5,
        VALUES_C = 6,
        ANSWER_D = 7,
        VALUES_D = 8,
    }

    [SerializeField]
    private StarterQuizQuestions m_questionsData;

    [SerializeField]
    private TextAsset m_csvFile;

    [MenuItem("Phantoms/Parse Starter Questions CSV")]
    public static void ShowWindow()
    {
        GetWindow<ParseStarterQuestionsCSV>("Parse Quest CSV");
    }

    private void OnGUI()
    {
        m_questionsData = (StarterQuizQuestions)EditorGUILayout.ObjectField(m_questionsData, typeof(StarterQuizQuestions), true);
        m_csvFile = (TextAsset)EditorGUILayout.ObjectField(m_csvFile, typeof(TextAsset), true);

        if (GUILayout.Button("Update"))
        {
            Parse();
        }
    }

    private void Parse()
    {
        if (m_csvFile == null)
        {
            Debug.Log("Assign a CSV dummy");
            return;
        }

        if (m_questionsData == null)
        {
            Debug.Log("Assign some data silly");
            return;
        }

        m_questionsData.Questions.Clear();

        List<List<string>> parsedData = ParseCSVUtil.ParseData(m_csvFile);

        // The first line is just the name  of the columns; start with the second line
        for (int i = 1; i < parsedData.Count; i++)
        {
            // First some simple error checking - make sure that there is at least 5 entries, as there should be. (question + 2 answers and 2 values)
            if (parsedData[i].Count < 5)
            {
                Debug.LogError("Line " + i + " is not long enough");
                continue;
            }

            List<string> stringData = parsedData[i];

            StarterQuizQuestions.StarterQuestion question = new StarterQuizQuestions.StarterQuestion();

            question.Question = stringData[0];

            int stringIndex = 1;
            while (stringIndex < (stringData.Count - 1))
            {
                StarterQuizQuestions.StarterQuestionAnswer answer = GetAnswer(stringData[stringIndex], stringData[stringIndex + 1]);
                question.Answers.Add(answer);
                stringIndex += 2;
            }

            m_questionsData.Questions.Add(question);
        }

        EditorUtility.SetDirty(m_questionsData);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private StarterQuizQuestions.StarterQuestionAnswer GetAnswer(string answerText, string valuesText)
    {
        StarterQuizQuestions.StarterQuestionAnswer answer = new StarterQuizQuestions.StarterQuestionAnswer();
        answer.Answer = answerText;

        string[] valuesTextList = valuesText.Trim().Split(',');

        foreach (string valueText in valuesTextList)
        {
            valueText.Trim();

            int split = 0;
            for (int i = 0; i < valueText.Length; i++)
            {
                if (valueText[i] == '+' || valueText[i] == '-')
                {
                    split = i;
                    break;
                }
            }
            string id = valueText.Substring(0, split);
            int value = 0;
            int.TryParse(valueText.Substring(split), out value);

            if (value != 0)
            {
                StarterQuizQuestions.StarterQuestionValue questionValue = new StarterQuizQuestions.StarterQuestionValue();
                questionValue.ID = id;
                questionValue.Value = value;
                answer.Values.Add(questionValue);
            }
        }


        return answer;
    }
}
