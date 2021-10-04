using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleScreenManager : MonoBehaviour
{
    [SerializeField]
    [Scene]
    private string m_starterScene = null;

    [SerializeField]
    private string m_startingMusicID = null;

    [SerializeField]
    private string m_startingAmbienceID = null;

    [SerializeField]
    private Image m_backing = null;
    [SerializeField]
    private CanvasGroup m_buttons = null;

    [SerializeField]
    private float m_backingFadeOutTime = 1f;
    [SerializeField]
    private float m_buttonFadeInTime = 0.3f;


    private Coroutine m_currentSequence = null;

    private void Awake()
    {
        m_backing.gameObject.SetActive(true);
        Color backingColor = m_backing.color;
        backingColor.a = 1f;
        m_backing.color = backingColor;
        m_buttons.gameObject.SetActive(false);
        m_currentSequence = StartCoroutine(StartingSequence());
    }

    private IEnumerator StartingSequence()
    {
        yield return SceneManager.LoadSceneAsync(m_starterScene, LoadSceneMode.Additive);
        yield return null;
        AudioManager.PlayMusic(m_startingMusicID);
        AudioManager.PlayAmbience(m_startingAmbienceID, 1f, m_backingFadeOutTime + m_buttonFadeInTime);
        OverworldManager.Instance.SetOverworldActive(false);

        float currentTime = 0f;
        while (currentTime < m_backingFadeOutTime)
        {
            Color backingColor = m_backing.color;
            backingColor.a = 1f - Mathf.Clamp01(currentTime / m_backingFadeOutTime);
            m_backing.color = backingColor;
            yield return null;
            currentTime += Time.deltaTime;
        }
        m_backing.gameObject.SetActive(false);
        m_buttons.gameObject.SetActive(true);
        currentTime = 0f;
        while (currentTime < m_buttonFadeInTime)
        {
            m_buttons.alpha = Mathf.Clamp01(currentTime / m_buttonFadeInTime);
            yield return null;
            currentTime += Time.deltaTime;
        }

        m_buttons.alpha = 1f;
    }
}
