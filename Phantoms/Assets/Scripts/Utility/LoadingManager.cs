using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{

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

    private const string LOADING_SCENE = "Assets/Scenes/Loading Scene.unity";
    private const string LOADING_SCREEN_PREFAB = "Loading Screen";

    private int m_loadingSceneIndex = 0;

    private bool m_loading = false;
    public static bool Loading { get { return Instance.m_loading; } }

    private GameObject m_loadingScreen = null;

    ////////////////////////////////////////////////////////////////
    /// Public functions to be called for scene loading behavior ///
    ////////////////////////////////////////////////////////////////

    // Public static function called to load into a scene, with a given scene index.
    public static void LoadScene(int sceneIndex)
    {
        Instance.LoadSceneInternal(sceneIndex, true);
    }

    // Public static function called to load into a scene, with a given scene name.
    public static void LoadSceneByName(string sceneName)
    {
        //LoadScene( .GetSceneByName(sceneName).buildIndex);
    }

    // Public static function called to load into a scene, with a given scene name.
    public static void LoadSceneByPath(string scenePath)
    {
        LoadScene(SceneUtility.GetBuildIndexByScenePath(scenePath));
    }

    public static void LoadSceneNoAnimation(string scenePath)
    {
        Instance.LoadSceneInternal(SceneUtility.GetBuildIndexByScenePath(scenePath), false);
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

    private void LoadSceneInternal(int sceneIndex, bool showLoadingScreen)
    {
        if (!m_loading)
        {
            m_loading = true;
            StartCoroutine(LoadSceneBackend(sceneIndex, showLoadingScreen));
        }
    }

    private IEnumerator LoadSceneBackend(int sceneIndex, bool showLoadingScreen = true)
    {
        PixelCrushers.DialogueSystem.DialogueManager.StopConversation();
        //PixelCrushers.DialogueSystem.DialogueManager.instance.displaySettings.subtitleSettings.continueButton = PixelCrushers.DialogueSystem.DisplaySettings.SubtitleSettings.ContinueButtonMode.Always;
        
        if (showLoadingScreen)
        {
            m_loadingScreen.SetActive(true);
        }

        SceneManager.LoadScene(m_loadingSceneIndex);

        while (SceneManager.GetActiveScene().buildIndex != m_loadingSceneIndex)
        {
            yield return null;
        }

        AsyncOperation load = SceneManager.LoadSceneAsync(sceneIndex);
        yield return load;

        yield return null;

        if (showLoadingScreen)
        {
            m_loadingScreen.SetActive(false);
        }
        m_loading = false;
    }
}
