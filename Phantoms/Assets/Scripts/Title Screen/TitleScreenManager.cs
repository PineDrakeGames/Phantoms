using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour
{
    [SerializeField]
    [Scene]
    private string m_starterScene = null;

    private Coroutine m_currentSequence = null;

    private void Awake()
    {
        m_currentSequence = StartCoroutine(StartingSequence());
    }

    private IEnumerator StartingSequence()
    {
        yield return SceneManager.LoadSceneAsync(m_starterScene, LoadSceneMode.Additive);
        OverworldManager.Instance.SetOverworldActive(false);
    }
}
