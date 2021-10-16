using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class TitleScreenManager : MonoBehaviour
{
    /////////////////////////
    /// Serialized Fields ///
    /////////////////////////
    [Header("Main Menu References")]
    [SerializeField]
    private Button m_startButton = null;
    [SerializeField]
    private Button m_continueButton = null;
    [SerializeField]
    private Button m_loadSavesButton = null;
    [SerializeField]
    private CanvasGroup m_mainMenuButtons = null;

    [Header("Save Slot Menu References")]
    [SerializeField]
    private CanvasGroup m_saveSlotButtons = null;
    [SerializeField]
    private Button m_deleteSaveButton = null;
    [SerializeField]
    private Button m_startSaveButton = null;
    [SerializeField]
    private TMP_Text m_startSaveText = null;

    [Header("Other References")]
    [SerializeField]
    private StarterQuizManager m_starterQuizManager = null;

    [SerializeField]
    private GameObject m_titleScreenParent = null;

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
    private float m_backingFadeOutTime = 1f;
    [SerializeField]
    private float m_buttonFadeInTime = 0.3f;


    private Coroutine m_currentSequence = null;

    private TitleMenuSaveSlot m_currentSlot = null;

    ///////////////////////
    /// Unity Functions ///
    ///////////////////////
    private void Awake()
    {
        m_backing.gameObject.SetActive(true);
        Color backingColor = m_backing.color;
        backingColor.a = 1f;
        m_backing.color = backingColor;
        m_mainMenuButtons.gameObject.SetActive(false);
        m_saveSlotButtons.gameObject.SetActive(false);

        DataManager.CreateInstance();
    }

    private void Start()
    {
        CheckSaveData();
        m_currentSequence = StartCoroutine(StartingSequence());
    }

    ///////////////////////////////
    /// Public Button Functions ///
    ///////////////////////////////

    public void StartNewGame(int slotNumber)
    {
        SaveDataManager.CurrentSaveSlot = slotNumber;

        m_titleScreenParent.SetActive(false);

        m_starterQuizManager.StartQuiz();
    }

    public void ContinueFromLastSave()
    {
        // Check to see if there is save data in the 
        int slot = Mathf.Clamp(DataManager.Instance.Settings.LastSaveSlotPlayed, 0, SaveSlotManager.NUM_SLOTS);
        if (!PixelCrushers.SaveSystem.HasSavedGameInSlot(slot))
        {
            for (int i = 0; i < SaveSlotManager.NUM_SLOTS; i++)
            {
                if (PixelCrushers.SaveSystem.HasSavedGameInSlot(i))
                {
                    slot = i;
                    break;
                }
            }
        }

        SaveDataManager.CurrentSaveSlot = slot;
        PixelCrushers.SaveSystem.LoadFromSlot(SaveDataManager.CurrentSaveSlot);
    }

    public void SelectSaveSlot(TitleMenuSaveSlot slot)
    {
        if (m_currentSlot == slot) { return; }
        if (m_currentSlot != null)
        {
            m_currentSlot.SetState(TitleMenuSaveSlot.SaveSlotState.DEFAULT);
        }
        m_currentSlot = slot;
        if (slot != null)
        {
            m_startSaveButton.interactable = true;
            if (PixelCrushers.SaveSystem.HasSavedGameInSlot(slot.SlotNumber))
            {
                m_startSaveText.text = "Continue Save";
                m_deleteSaveButton.interactable = true;
            }
            else
            {
                m_startSaveText.text = "Start New Game";
                m_deleteSaveButton.interactable = false;
            }
        }
        else
        {
            m_startSaveText.text = "Select a Save";
            m_deleteSaveButton.interactable = false;
            m_startSaveButton.interactable = false;
        }
    }

    public void DeselectSaveSlot()
    {
        SelectSaveSlot(null);
    }

    public void StartSave()
    {
        if (m_currentSlot != null)
        {
            if (PixelCrushers.SaveSystem.HasSavedGameInSlot(m_currentSlot.SlotNumber))
            {
                SaveDataManager.CurrentSaveSlot = m_currentSlot.SlotNumber;
                PixelCrushers.SaveSystem.LoadFromSlot(SaveDataManager.CurrentSaveSlot);
            }
            else
            {
                StartNewGame(m_currentSlot.SlotNumber);
            }
        }
    }

    public void DeleteSave()
    {
        if (m_currentSlot != null)
        {
            if (PixelCrushers.SaveSystem.HasSavedGameInSlot(m_currentSlot.SlotNumber))
            {
                PixelCrushers.SaveSystem.DeleteSavedGameInSlot(m_currentSlot.SlotNumber);
                SaveSlotManager.CleanUpSlots();
                m_currentSlot.SetDisplay();
                SelectSaveSlot(null);
                SelectSaveSlot(m_currentSlot);
            }
        }
    }

    public void GoToSaveSlots()
    {
        m_mainMenuButtons.gameObject.SetActive(false);
        m_saveSlotButtons.gameObject.SetActive(true);
        DeselectSaveSlot();
    }

    public void ReturnToMainMenu()
    {
        m_mainMenuButtons.gameObject.SetActive(true);
        m_saveSlotButtons.gameObject.SetActive(false);
    }

    //////////////////////////////////////////////
    /// Private Helper Functions + Coroutines! ///
    //////////////////////////////////////////////
    private void CheckSaveData()
    {
        // Load the settings first (If they are there)
        SaveSettingsManager.Load();

        // Load the save slots! Basically, check if we have save data - if we do, set up the save slot buttons,
        // Otherwise set up the 'first time screen' basically.
        SaveSlotManager.Load();
        SaveSlotManager.CleanUpSlots();

        bool hasSaveData = false; // Start by assuming we don't have save data, then go through the slots (if there are any)

        // TODO: Just because we don't have the slot data, we might still have the actual save data - 
        // If thats the case, just make some slot data but leave the rest of the info blank for now
        for (int i = 0; i < SaveSlotManager.NUM_SLOTS; i++)
        {
            SaveSlot slot = SaveSlotManager.GetSlotData(i);
            if (slot != null)
            {
                if (PixelCrushers.SaveSystem.HasSavedGameInSlot(slot.SlotNumber))
                {
                    hasSaveData = true;
                    break;
                }
            }
        }

        if (hasSaveData)
        {
            m_startButton.gameObject.SetActive(false);
            m_continueButton.gameObject.SetActive(true);
            m_loadSavesButton.gameObject.SetActive(true);
        }
        else
        {
            m_startButton.gameObject.SetActive(true);
            m_continueButton.gameObject.SetActive(false);
            m_loadSavesButton.gameObject.SetActive(false);
        }
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
        m_mainMenuButtons.gameObject.SetActive(true);
        currentTime = 0f;
        while (currentTime < m_buttonFadeInTime)
        {
            m_mainMenuButtons.alpha = Mathf.Clamp01(currentTime / m_buttonFadeInTime);
            yield return null;
            currentTime += Time.deltaTime;
        }

        m_mainMenuButtons.alpha = 1f;
    }
}
