using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

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
    public static bool Loading
    {
        get
        {
            return Instance.m_loading;
        }
        set
        {
            Instance.m_loading = value;
            if (Instance.m_loading)
            {
                Player.PlayerInputEnabled = false;
            }
            else
            {
                Player.PlayerInputEnabled = true;
            }
        }
    }
    public static UnityEvent NewSceneLoaded = new UnityEvent();

    private GameObject m_loadingScreen = null;
    private Animator m_loadingScreenAnimator = null;

    // Stuff for the current overworld
    private List<GameObject> m_overworldSceneItems = new List<GameObject>();
    private Scene m_lastOverworldScene;
    public Scene LastOverworldScene { get { return m_lastOverworldScene; } }
    private AudioClip m_currentOverworldMusic = null;


    ///////////////////////////////////////////////
    /// Enum and values to identify scene types ///
    ///////////////////////////////////////////////

    public enum LoadingScreenDirection
    {
        UP = 1,
        RIGHT = 2,
        DOWN = 3,
        LEFT = 4
    }

    public static LoadingScreenDirection CurrentLoadDirection = LoadingScreenDirection.UP;

    public static Vector2 LoadWalkDirection = Vector2.zero;

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

    public static void LoadBattle(string scenePath)
    {
        LoadBattle(SceneUtility.GetBuildIndexByScenePath(scenePath));
    }

    public static void LoadBattle(int sceneIndex)
    {
        Instance.LoadBattleInternal(sceneIndex);
    }

    public static void ReturnFromBattle()
    {
        Instance.ReturnFromBattleInternal();
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
        m_loadingScreenAnimator = m_loadingScreen.GetComponentInChildren<Animator>();
        m_loadingScreen.SetActive(false);
        m_lastOverworldScene = SceneManager.GetActiveScene();
    }


    // Internal function called by the static scene load functions to access any instanced variables.
    private void LoadSceneInternal(int sceneIndex)
    {
        if (!Loading)
        {
            Loading = true;
            StartCoroutine(LoadSceneBackend(sceneIndex));
        }
    }

    private void LoadSceneInternal(int sceneIndex, SceneType newSceneType = SceneType.OVERWORLD, bool showLoadingScreen = true)
    {
        if (!Loading)
        {
            Loading = true;
            StartCoroutine(LoadSceneBackend(sceneIndex, newSceneType, showLoadingScreen));
        }
    }

    private void LoadBattleInternal(int sceneIndex)
    {
        if (!Loading)
        {
            Loading = true;
            StartCoroutine(LoadIntoBattleBackend(sceneIndex));
        }
    }

    private void ReturnFromBattleInternal()
    {

        if (!Loading)
        {
            if (m_lastOverworldScene == SceneManager.GetActiveScene())
            {
                LoadSceneInternal(0);
            }
            else
            {
                Loading = true;
                StartCoroutine(ReturnFromBattleBackend());
            }
        }
    }

    private Scene GetLoadedSceneByIndex(int buildIndex)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.buildIndex == buildIndex)
            {
                return scene;
            }
        }
        return SceneManager.GetActiveScene(); ;
    }

    private IEnumerator LoadSceneBackend(int sceneIndex, SceneType newSceneType = SceneType.OVERWORLD, bool showLoadingScreen = true)
    {
        PixelCrushers.DialogueSystem.DialogueManager.StopConversation();
        //PixelCrushers.DialogueSystem.DialogueManager.instance.displaySettings.subtitleSettings.continueButton = PixelCrushers.DialogueSystem.DisplaySettings.SubtitleSettings.ContinueButtonMode.Always;

        // Do Loading screen!
        if (showLoadingScreen)
        {
            yield return ShowLoadingScreen((int)CurrentLoadDirection);
        }

        // Disabling all things from all types of scenes

        // Load into the loading scene, and wait until we are there.
        SceneManager.LoadScene(m_loadingSceneIndex);
        while (SceneManager.GetActiveScene().buildIndex != m_loadingSceneIndex)
        {
            yield return null;
        }
        OverworldManager.Instance.SetOverworldActive(false);


        // Load the new scene, and wait for that scene to finish loading.
        AsyncOperation load = SceneManager.LoadSceneAsync(sceneIndex);
        yield return load;

        // Check what to do for specific new scene types
        switch (newSceneType)
        {
            case SceneType.OTHER:
                break;
            case SceneType.OVERWORLD:
                OverworldManager.Instance.PlaceOverworldObjects();
                OverworldManager.Instance.SetOverworldActive(true);
                m_lastOverworldScene = SceneManager.GetActiveScene();
                break;
            case SceneType.BATTLE:
                break;
        }

        // Update the current scene type
        m_currentSceneType = newSceneType;

        // Show the unloading screen
        NewSceneLoaded.Invoke();
        if (showLoadingScreen)
        {
            yield return HideLoadingScreen((int)CurrentLoadDirection);
        }

        // Clean Up!
        Loading = false;
    }

    private IEnumerator LoadIntoBattleBackend(int sceneIndex, bool showLoadingScreen = true)
    {
        LoadWalkDirection = Vector2.zero;
        AudioManager.StopMusic();
        AudioManager.SetAmbienceVolume(0.1f);
        if (showLoadingScreen)
        {
            yield return ShowLoadingScreen(5);
        }
        yield return null;


        m_overworldSceneItems.Clear();

        foreach (GameObject rootObject in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            if (rootObject.activeSelf && !OverworldManager.Instance.BattlePersistantInstances.Contains(rootObject))
            {
                rootObject.SetActive(false);
                m_overworldSceneItems.Add(rootObject);
            }
        }

        m_lastOverworldScene = SceneManager.GetActiveScene();
        m_currentOverworldMusic = AudioManager.CurrentMusicClip;

        yield return SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive);
        OverworldManager.Instance.SetOverworldActive(false);

        SceneManager.SetActiveScene(GetLoadedSceneByIndex(sceneIndex));

        m_currentSceneType = SceneType.BATTLE;
        NewSceneLoaded.Invoke();

        if (showLoadingScreen)
        {
            yield return HideLoadingScreen(5);
        }

        Loading = false;
    }

    private IEnumerator ReturnFromBattleBackend()
    {
        yield return ShowLoadingScreen(0);

        yield return null;

        AudioManager.StopMusic();

        AsyncOperation unload = SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());

        yield return unload;

        OverworldManager.Instance.SetOverworldActive(true);
        foreach (GameObject rootObject in m_overworldSceneItems)
        {
            if (rootObject != null)
            {
                rootObject.SetActive(true);
            }
        }

        AudioManager.PlayMusic(m_currentOverworldMusic);

        SceneManager.SetActiveScene(m_lastOverworldScene);
        m_currentSceneType = SceneType.OVERWORLD;
        NewSceneLoaded.Invoke();
        yield return HideLoadingScreen(0);

        Loading = false;
    }


    // Helper Coroutines, for things that all the different loads need.
    private IEnumerator ShowLoadingScreen(int direction)
    {
        m_loadingScreen.SetActive(true);
        if (m_loadingScreenAnimator)
        {
            m_loadingScreenAnimator.SetInteger("Direction", direction);
            m_loadingScreenAnimator.SetBool("Visible", true);

            Player.SetCharacterInput(LoadWalkDirection);

            // TODO: Wait for animation to finish instead!
            yield return new WaitForSeconds(0.35f);
        }
    }

    private IEnumerator HideLoadingScreen(int direction)
    {
        if (m_loadingScreenAnimator)
        {
            m_loadingScreenAnimator.SetInteger("Direction", direction);
            m_loadingScreenAnimator.SetBool("Visible", false);
            Player.SetCharacterInput(LoadWalkDirection);

            // TODO: Wait for animation to finish instead!
            yield return new WaitForSeconds(0.35f);
        }
        m_loadingScreen.SetActive(false);
    }
}
