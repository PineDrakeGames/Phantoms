using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////
/// Inspector Area ///
//////////////////////
#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
using UnityEngine.SceneManagement;

[CustomEditor(typeof(OverworldSceneTable))]
public class OverworldSceneTableEditor : Editor
{
    SerializedProperty scenes;

    //The Reorderable list we will be working with
    ReorderableList list;

    private void OnEnable()
    {
        MakeReorderableList();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        list.DoLayoutList();

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        OverworldSceneTable sceneTable = (OverworldSceneTable)target;
        if (GUILayout.Button("Add All"))
        {
            sceneTable.AddAllSceneLocations();
            MakeReorderableList();
        }
        if (GUILayout.Button("Replace All"))
        {
            sceneTable.SceneToLocations.Clear();
            sceneTable.AddAllSceneLocations();
            MakeReorderableList();
        }
        EditorGUILayout.EndHorizontal();

        serializedObject.ApplyModifiedProperties();
    }

    private void MakeReorderableList()
    {
        //Gets the wave property in WaveManager so we can access it. 
        scenes = serializedObject.FindProperty("m_scenes");

        //Initialises the ReorderableList. We are creating a Reorderable List from the "wave" property. 
        //In this, we want a ReorderableList that is draggable, with a display header, with add and remove buttons        
        list = new ReorderableList(serializedObject, scenes, true, true, true, true);
        list.drawElementCallback = DrawListItems;
        list.drawHeaderCallback = DrawListHeader;
        list.elementHeightCallback = DrawListHeight;
        list.onAddCallback = OnAddCallback;
    }

    private void DrawListItems(Rect rect, int index, bool isActive, bool isFocused)
    {
        SerializedProperty element = list.serializedProperty.GetArrayElementAtIndex(index); //The element in the list
        rect.y += 2;

        // Create a property field and label field for each property.
        SerializedProperty elementName = element.FindPropertyRelative("Scene");

        EditorGUI.PropertyField(
            new Rect(rect.x, rect.y, 200, EditorGUIUtility.singleLineHeight),
            elementName,
            GUIContent.none
        );

        rect.y += EditorGUIUtility.singleLineHeight * 1.2f;

        // The 'level' property
        // The label field for level (width 100, height of a single line)

        EditorGUI.PropertyField(position:
            new Rect(rect.x += 10, rect.y, Screen.width * .8f, height: EditorGUIUtility.singleLineHeight), element.FindPropertyRelative("SceneLocations"), new GUIContent("Locations"), includeChildren: true);
    }

    private void DrawListHeader(Rect rect)
    {
        string name = "Scene";
        EditorGUI.LabelField(rect, name);
    }

    private float DrawListHeight(int index)
    {
        var element = list.serializedProperty.GetArrayElementAtIndex(index);

        element.isExpanded = true;

        float propertyHeight = EditorGUI.GetPropertyHeight(list.serializedProperty.GetArrayElementAtIndex(index), true);
        float spacing = EditorGUIUtility.singleLineHeight / 8;

        return propertyHeight + spacing;
    }

    private void OnAddCallback(ReorderableList list)
    {
        var index = list.serializedProperty.arraySize;
        list.serializedProperty.arraySize++;
        list.index = index;
        var element = list.serializedProperty.GetArrayElementAtIndex(index);

        element.FindPropertyRelative("Scene").stringValue = null;
        element.FindPropertyRelative("SceneLocations").arraySize = 0;
    }
}
#endif

/////////////////////////////////
/// Scene location Data Class ///
/////////////////////////////////
[System.Serializable]
public class SceneLocationData
{
    [Scene]
    public string Scene = null;

    public List<string> SceneLocations = new List<string>();
}

//////////////////////////
/// Scene Table Object ///
//////////////////////////
[CreateAssetMenu(menuName = "Phantoms/Scene Table")]
public class OverworldSceneTable : ScriptableObject, ISerializationCallbackReceiver
{
    [SerializeField]
    private List<SceneLocationData> m_scenes = new List<SceneLocationData>();

    public Dictionary<string, List<string>> SceneToLocations = new Dictionary<string, List<string>>();

    public void OnBeforeSerialize()
    {
        m_scenes = new List<SceneLocationData>();
        foreach (KeyValuePair<string, List<string>> kvp in SceneToLocations)
        {
            SceneLocationData locData = new SceneLocationData();
            locData.Scene = kvp.Key;
            locData.SceneLocations = new List<string>(kvp.Value);
            m_scenes.Add(locData);
        }
    }

    public void OnAfterDeserialize()
    {
        SceneToLocations = new Dictionary<string, List<string>>();

        foreach (SceneLocationData data in m_scenes)
        {
            if (SceneToLocations.ContainsKey(data.Scene) && !string.IsNullOrEmpty(data.Scene))
            {
                SceneToLocations[data.Scene].AddRange(data.SceneLocations);
            }
            else
            {
                SceneToLocations[data.Scene] = new List<string>(data.SceneLocations);
            }
        }
    }

#if UNITY_EDITOR
    /////////////////////////////
    /// Editor Only Functions ///
    /////////////////////////////

    public void AddAllSceneLocations()
    {
        var originalScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().path;
        /*
                for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
                {
                    // TODO: Go through all of the scenes!
                    Scene scene = UnityEditor.SceneManagement.EditorSceneManager.get
                    Debug.Log(scene.name + ", " + scene.buildIndex);
                    GameObject[] rootGameObjects = scene.GetRootGameObjects();
                    foreach(GameObject rootObject in rootGameObjects)
                    {
                        OverworldSceneEnterTrigger[] enterTriggers = rootObject.GetComponentsInChildren<OverworldSceneEnterTrigger>();
                        foreach (OverworldSceneEnterTrigger enterTrigger in enterTriggers)
                        {
                            if (!string.IsNullOrEmpty(enterTrigger.LocationID))
                            {
                                if (!SceneToLocations.ContainsKey(scene.path))
                                {
                                    SceneToLocations[scene.path] = new List<string>();
                                }
                                if (!SceneToLocations[scene.path].Contains(enterTrigger.LocationID))
                                {
                                    SceneToLocations[scene.path].Add(enterTrigger.LocationID);
                                }
                            }
                        }
                    }
                }
                */

        foreach (var sceneGUID in AssetDatabase.FindAssets("t:Scene", new string[] { "Assets" }))
        {
            var scenePath = AssetDatabase.GUIDToAssetPath(sceneGUID);

            UnityEditor.SceneManagement.EditorSceneManager.OpenScene(scenePath);
            var scene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene();

            GameObject[] rootGameObjects = scene.GetRootGameObjects();
            foreach (GameObject rootObject in rootGameObjects)
            {
                OverworldSceneEnterTrigger[] enterTriggers = rootObject.GetComponentsInChildren<OverworldSceneEnterTrigger>();
                foreach (OverworldSceneEnterTrigger enterTrigger in enterTriggers)
                {
                    if (!string.IsNullOrEmpty(enterTrigger.LocationID))
                    {
                        if (!SceneToLocations.ContainsKey(scene.path))
                        {
                            SceneToLocations[scene.path] = new List<string>();
                        }
                        if (!SceneToLocations[scene.path].Contains(enterTrigger.LocationID))
                        {
                            SceneToLocations[scene.path].Add(enterTrigger.LocationID);
                        }
                    }
                }
            }
        }

        if (originalScene != "") UnityEditor.SceneManagement.EditorSceneManager.OpenScene(originalScene);

    }


#endif
}
