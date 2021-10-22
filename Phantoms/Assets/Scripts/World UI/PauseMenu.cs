using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject m_pauseMenuParent = null;

    [SerializeField]
    [Tooltip("In the overworld, we check for pausing elsewhere, but in battle we check here.")]
    private bool m_checkForPauseInput = false;

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
        if (s_instance == null)
        {
            s_instance = this;
            UnPause();
        }
        m_pauseMenuParent.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && m_checkForPauseInput)
        {
            TogglePause();
        }
    }

    public void Pause()
    {
        if (!LoadingManager.Loading)
        {
            Time.timeScale = 0f;
            m_pauseMenuParent.SetActive(true);
            m_paused = true;

            Player.PlayerInputEnabled = false;
        }
    }

    public void UnPause()
    {
        Time.timeScale = 1f;
        m_pauseMenuParent.SetActive(false);
        m_paused = false;
        Player.PlayerInputEnabled = true;
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

    public void ReturnToMainMenu()
    {
        LoadingManager.LoadScene(0);
        UnPause();
    }
}
