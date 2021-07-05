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


    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawDefaultInspector();

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        OverworldSceneTable sceneTable = (OverworldSceneTable)target;
        if (GUILayout.Button("Add All"))
        {
            sceneTable.AddAllSceneLocations();
        }
        if (GUILayout.Button("Replace All"))
        {
            sceneTable.SceneToLocations.Clear();
            sceneTable.AddAllSceneLocations();
        }
        EditorGUILayout.EndHorizontal();
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
