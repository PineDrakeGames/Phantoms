using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(OverworldSceneExitTrigger))]
public class OverworldSceneExitTriggerEditor : Editor
{

    SerializedProperty sceneToLoad;
    SerializedProperty loadLocationID;
    SerializedProperty exitDirection;
    SerializedProperty playerWalkDirection;

    private string[] locationIDOptions = null;
    private int IDIndex = 0;

    private void OnEnable()
    {
        sceneToLoad = serializedObject.FindProperty("m_sceneToLoad");
        loadLocationID = serializedObject.FindProperty("m_loadLocationID");
        exitDirection = serializedObject.FindProperty("m_exitDirection");
        playerWalkDirection = serializedObject.FindProperty("m_playerWalkDirection");

        GetLocationOptions();
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(sceneToLoad);
        if (EditorGUI.EndChangeCheck())
        {
            GetLocationOptions();
        }

        IDIndex = EditorGUILayout.Popup(IDIndex, locationIDOptions);
        loadLocationID.stringValue = locationIDOptions[IDIndex];

        EditorGUILayout.PropertyField(exitDirection);
        EditorGUILayout.PropertyField(playerWalkDirection);

        serializedObject.ApplyModifiedProperties();
    }

    private void GetLocationOptions()
    {
        OverworldSceneTable table = Resources.Load<OverworldSceneTable>("Overworld Scene Table");
        string currentScene = sceneToLoad.stringValue;

        if (table.SceneToLocations.ContainsKey(currentScene))
        {
            locationIDOptions = table.SceneToLocations[currentScene].ToArray();
            if (locationIDOptions.Length == 0)
            {
                locationIDOptions = new string[1];
                locationIDOptions[0] = "No Locations Available";
                IDIndex = 0;
            }
            else
            {
                IDIndex = 0;
                for (int i = 0; i < locationIDOptions.Length; i++)
                {
                    if (locationIDOptions[i] == loadLocationID.stringValue)
                    {
                        IDIndex = i;
                        break;
                    }
                }
            }
        }
        else
        {
            locationIDOptions = new string[1];
            locationIDOptions[0] = "No Locations Available";
            IDIndex = 0;
        }
    }
}

#endif

public class OverworldSceneExitTrigger : MonoBehaviour
{
    [SerializeField]
    [Scene]
    private string m_sceneToLoad = null;

    [SerializeField]
    private string m_loadLocationID = null;

    [Tooltip("The Loading screen will enter FROM the direction set - should set to the direction of the path you are entering.")]
    [SerializeField]
    private LoadingManager.LoadingScreenDirection m_exitDirection = LoadingManager.LoadingScreenDirection.RIGHT;

    [Tooltip("The direction the player will walk when touching the trigger")]
    [SerializeField]
    private Vector2 m_playerWalkDirection = Vector2.zero;
    

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            OverworldManager.Instance.SetLocationID(m_loadLocationID);
            LoadingManager.CurrentLoadDirection = m_exitDirection;
            LoadingManager.LoadWalkDirection = m_playerWalkDirection;
            LoadingManager.LoadScene(m_sceneToLoad, LoadingManager.SceneType.OVERWORLD);
        }
    }
}
