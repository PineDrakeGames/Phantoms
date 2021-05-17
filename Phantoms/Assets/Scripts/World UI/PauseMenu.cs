using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject m_pauseMenuParent = null;

    private static PauseMenu s_instance = null;
    public static PauseMenu Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = FindObjectOfType<PauseMenu>();
            }
            return s_instance;
        }
    }

    private bool m_paused = false;
    public bool Paused { get { return m_paused; } }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            s_instance = this;
            UnPause();
        }
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        m_pauseMenuParent.SetActive(true);
        m_paused = true;
    }

    public void UnPause()
    {
        Time.timeScale = 1f;
        m_pauseMenuParent.SetActive(false);
        m_paused = false;
    }

    public void TogglePause()
    {
        if (m_paused)
        {
            UnPause();
        }
        else
        {
            Pause();
        }
    }

    public void Quit()
    {
        Application.Quit();
    }
}
