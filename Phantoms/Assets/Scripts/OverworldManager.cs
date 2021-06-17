using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(OverworldManager))]
public class OverworldManagerEditor : Editor 
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        OverworldManager myTarget = (OverworldManager)target;
        GUILayout.Space(20f);
        if (GUILayout.Button("Instantiate Stuff"))
        {
            myTarget.EditorInitialize();
        }
    }
}
#endif

public class OverworldManager : MonoBehaviour
{
    /////////////////////
    /// Editor Fields ///
    /////////////////////

    //[Header("Scene-Specific References")]

    [Header("Instance References")]
    public GameObject PlayerInstance = null;
    public GameObject CanvasInstance = null;
    public GameObject CameraInstance = null;
    [HideInInspector]
    public List<GameObject> PersistantInstances = new List<GameObject>();
    [HideInInspector]
    public List<GameObject> BattlePersistantInstances = new List<GameObject>();

    [Header("Persistent prefabs")]
    [SerializeField]
    private GameObject m_playerPrefab = null;
    [SerializeField]
    private GameObject m_canvasPrefab = null;
    [SerializeField]
    private GameObject m_cameraPrefab = null;
    [SerializeField]
    [Tooltip("List of all prefabs that are needed in all scenes.")]
    private List<GameObject> m_persistentPrefabs = new List<GameObject>();
    [SerializeField]
    [Tooltip("List of all prefabs that are needed in all scenes, INCLUDING battle scenes.")]
    private List<GameObject> m_battlePersistentPrefabs = new List<GameObject>();


    /////////////////////////////
    /// Static Instance Stuff ///
    /////////////////////////////
    private static OverworldManager s_instance = null;
    public static OverworldManager Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = FindObjectOfType<OverworldManager>();
                if (s_instance == null)
                {
                    GameObject instance = Instantiate(Resources.Load(OVERWORLD_MANAGER_PREFAB, typeof(GameObject))) as GameObject;
                    instance.name = "Overworld Manager";
                    s_instance = instance.GetComponent<OverworldManager>();
                }
                if (s_instance)
                {
                    s_instance.Initialize();
                }
            }

            return s_instance;
        }
    }


    ///////////////////////////////////////
    /// Private variables and Constants ///
    ///////////////////////////////////////

    private const string OVERWORLD_MANAGER_PREFAB = "Overworld Manager";
    private string m_loadLocationID;

    private CameraController m_cameraController = null;
    public CameraController CamController { get { return m_cameraController; } }
    private PlayerController m_playerController = null;
    public PlayerController PlayerController { get { return m_playerController; } }

    ///////////////////////
    /// Unity Functions ///
    ///////////////////////
    private void Awake()
    {
        if (s_instance == null)
        {
            s_instance = this;
            Initialize();
        }
        else if (s_instance != this)
        {
            DeInitialize();
        }
    }

    ////////////////////////////////
    /// Public Utility functions ///
    ////////////////////////////////

    // Initializes everything needed for the overworld, if not already done.
    public void Initialize()
    {
        DontDestroyOnLoad(this.gameObject);

        if (PlayerInstance == null)
        {
            PlayerInstance = Instantiate(m_playerPrefab);
            // TODO: Place in the right location?
        }
        m_playerController = PlayerInstance.GetComponent<PlayerController>();
        DontDestroyOnLoad(PlayerInstance);

        if (CanvasInstance == null)
        {
            CanvasInstance = Instantiate(m_canvasPrefab);
            // TODO: Eventsystem as well
        }
        DontDestroyOnLoad(CanvasInstance);

        if (CameraInstance == null)
        {
            CameraInstance = Instantiate(m_cameraPrefab);
            // TODO: Set initial position at player
        }
        m_cameraController = CameraInstance.GetComponent<CameraController>();
        DontDestroyOnLoad(CameraInstance);
        foreach(GameObject persistant in m_persistentPrefabs)
        {
            GameObject instance = Instantiate(persistant);
            DontDestroyOnLoad(instance);
            PersistantInstances.Add(instance);
        }
        foreach(GameObject persistant in m_battlePersistentPrefabs)
        {
            GameObject instance = Instantiate(persistant);
            DontDestroyOnLoad(instance);
            BattlePersistantInstances.Add(instance);
        }
    }

    // De-initialize everything (mostly used in cases where 2 Overworld managers exist)
    public void DeInitialize()
    {
        // Destroy any instances of persistant objects
        if (PlayerInstance) { Destroy (PlayerInstance); }
        if (CanvasInstance) { Destroy (CanvasInstance); }
        if (CameraInstance) { Destroy (CameraInstance); }
        
        PersistantInstances.Clear();
        BattlePersistantInstances.Clear();
        // Destroy this last
        Destroy(this.gameObject);
    }


    ///////////////////////////////////
    /// Public Transition Functions ///
    ///////////////////////////////////
    public void SetOverworldActive(bool active)
    {
        if (PlayerInstance) { PlayerInstance.SetActive(active); }
        if (CanvasInstance) { CanvasInstance.SetActive(active); }
        if (CameraInstance) { CameraInstance.SetActive(active); }
        foreach(GameObject instance in PersistantInstances)
        {
            if (instance != null)
            {
                instance.SetActive(active);
            }
        }
    }

    public void SetLocationID(string newLocationID)
    {
        m_loadLocationID = newLocationID;
    }

    public void PlaceOverworldObjects()
    {
        OverworldSceneEnterTrigger[] sceneEnterTriggers = FindObjectsOfType<OverworldSceneEnterTrigger>();

        bool foundTrigger = false;
        foreach(OverworldSceneEnterTrigger enterTrigger in sceneEnterTriggers)
        {
            if (enterTrigger.LocationID == m_loadLocationID)
            {
                // TODO: Have the enter trigger handle placing the player - just setting to position for now.
                m_playerController.Motor.SetPositionAndRotation(enterTrigger.transform.position, enterTrigger.transform.rotation);
                PlayerInstance.transform.position = enterTrigger.transform.position;
                LoadingManager.CurrentLoadDirection = enterTrigger.EnterDirection;

                PlayerRespawnManager.Instance.SetSafeRespawn(enterTrigger.transform.position);

                foundTrigger = true;
                break;
            }
        }

        if (!foundTrigger)
        {
            m_playerController.Motor.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        }

        m_cameraController.ResetCameraPosition();
    }

#if UNITY_EDITOR
    /////////////////////////////
    /// Editor Only Functions ///
    /////////////////////////////

    // Editor friendly version of the above
    public void EditorInitialize()
    {
        if (PlayerInstance == null)
        {
            PlayerInstance = PrefabUtility.InstantiatePrefab(m_playerPrefab) as GameObject;
        }
        if (CanvasInstance == null)
        {
            CanvasInstance = PrefabUtility.InstantiatePrefab(m_canvasPrefab) as GameObject;
        }
        if (CameraInstance == null)
        {
            CameraInstance = PrefabUtility.InstantiatePrefab(m_cameraPrefab) as GameObject;
        }

        EditorUtility.SetDirty(this);
        UnityEditor.SceneManagement.EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
    }
#endif
}
