using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    /////////////////////////////
    /// Static Instance Stuff ///
    /////////////////////////////
    private static LoadingManager s_instance = null;
    public static LoadingManager Instance
    {
        get
        {
            if (s_instance == null)
            {
                GameObject loadingObject = Instantiate(new GameObject());
                loadingObject.name = "Loading Manager";
                LoadingManager instance = loadingObject.AddComponent<LoadingManager>();
                DontDestroyOnLoad(loadingObject);
                loadingObject.name = "Loading Manager";

                instance.Initialize();
                s_instance = instance;
            }
            return s_instance;
        }
    }

    /////////////////////////
    /// Private variables ///
    /////////////////////////
    private const string LOADING_SCENE = "Assets/Scenes/Loading Scene.unity";
    private const string LOADING_SCREEN_PREFAB = "Loading Screen";

    private int m_loadingSceneIndex = 0;

    private bool m_loading = false;
    public static bool Loading { get { return Instance.m_loading; } }

    private GameObject m_loadingScreen = null;

    ///////////////////////////////////////////////
    /// Enum and values to identify scene types ///
    ///////////////////////////////////////////////

    public enum SceneType
    {
        OTHER = 0,
        OVERWORLD = 1,
        BATTLE = 2
    }

    private SceneType m_currentSceneType = SceneType.OTHER;

    ////////////////////////////////////////////////////////////////
    /// Public functions to be called for scene loading behavior ///
    ////////////////////////////////////////////////////////////////

    // Public static function called to load into a scene, with a given scene index.
    public static void LoadScene(int sceneIndex)
    {
        Instance.LoadSceneInternal(sceneIndex);
    }

    public static void LoadScene(int sceneIndex, SceneType newSceneType)
    {
        Instance.LoadSceneInternal(sceneIndex, newSceneType);
    }

    // Public static function called to load into a scene, with a given scene path.
    public static void LoadScene(string scenePath)
    {
        LoadScene(SceneUtility.GetBuildIndexByScenePath(scenePath));
    }

    public static void LoadScene(string scenePath, SceneType newSceneType)
    {
        LoadScene(SceneUtility.GetBuildIndexByScenePath(scenePath), newSceneType);
    }


    //////////////////////////////////////////////////////////////////
    /// private functions for internal scene loading functionality ///
    //////////////////////////////////////////////////////////////////

    // Internal function to initialize any static variables
    private void Initialize()
    {
        m_loading = false;
        m_loadingSceneIndex = SceneUtility.GetBuildIndexByScenePath(LOADING_SCENE);

        m_loadingScreen = Instantiate(Resources.Load(LOADING_SCREEN_PREFAB, typeof(GameObject))) as GameObject;
        DontDestroyOnLoad(m_loadingScreen);
        m_loadingScreen.SetActive(false);
    }


    // Internal function called by the static scene load functions to access any instanced variables.
    private void LoadSceneInternal(int sceneIndex)
    {
        if (!m_loading)
        {
            m_loading = true;
            StartCoroutine(LoadSceneBackend(sceneIndex));
        }
    }

    private void LoadSceneInternal(int sceneIndex, SceneType newSceneType = SceneType.OTHER, bool showLoadingScreen = true)
    {
        if (!m_loading)
        {
            m_loading = true;
            StartCoroutine(LoadSceneBackend(sceneIndex,newSceneType, showLoadingScreen));
        }
    }

    private IEnumerator LoadSceneBackend(int sceneIndex, SceneType newSceneType = SceneType.OTHER, bool showLoadingScreen = true)
    {
        PixelCrushers.DialogueSystem.DialogueManager.StopConversation();
        //PixelCrushers.DialogueSystem.DialogueManager.instance.displaySettings.subtitleSettings.continueButton = PixelCrushers.DialogueSystem.DisplaySettings.SubtitleSettings.ContinueButtonMode.Always;
        
        if (showLoadingScreen)
        {
            m_loadingScreen.SetActive(true);
        }

        // Disabling all things from all types of scenes
        OverworldManager.Instance.SetOverworldActive(false);


        SceneManager.LoadScene(m_loadingSceneIndex);
        while (SceneManager.GetActiveScene().buildIndex != m_loadingSceneIndex)
        {
            yield return null;
        }

        AsyncOperation load = SceneManager.LoadSceneAsync(sceneIndex);
        yield return load;

        switch(newSceneType)
        {
            case SceneType.OTHER:
                break;
            case SceneType.OVERWORLD:
                OverworldManager.Instance.PlaceOverworldObjects();
                OverworldManager.Instance.SetOverworldActive(true);
                break;
            case SceneType.BATTLE:
                break;
        }

        yield return null;

        if (showLoadingScreen)
        {
            m_loadingScreen.SetActive(false);
        }
        m_loading = false;
    }
}
