using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SceneLocationData
{
    [Scene]
    public string Scene = null;

    public List<string> SceneLocations = new List<string>();
}

[CreateAssetMenu(menuName = "Phantoms/Scene Table")]
public class OverworldSceneTable : ScriptableObject, ISerializationCallbackReceiver
{
    [SerializeField]
    private List<SceneLocationData> m_scenes = new List<SceneLocationData>();

    public Dictionary<string, List<string>> SceneToLocations = new Dictionary<string, List<string>>();

    public void OnBeforeSerialize()
    {
        m_scenes = new List<SceneLocationData>();
        foreach(KeyValuePair<string, List<string>> kvp in SceneToLocations)
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
            if (SceneToLocations.ContainsKey(data.Scene))
            {
                SceneToLocations[data.Scene].AddRange(data.SceneLocations);
            }
            else
            {
                SceneToLocations[data.Scene] = new List<string>(data.SceneLocations);
            }
        }
    }
}
