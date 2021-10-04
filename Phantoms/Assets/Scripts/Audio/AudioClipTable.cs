using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(AudioClipTable))]
public class AudioClipTableEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawDefaultInspector();

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Remove Duplicates"))
        {
            AudioClipTable clipTable = (AudioClipTable)target;
            clipTable.RemoveDuplicates();
        }

        EditorGUILayout.EndHorizontal();
    }
}
#endif

[CreateAssetMenu(menuName = "Phantoms/Audio/Audio Clip Table")]
public class AudioClipTable : ScriptableObject, ISerializationCallbackReceiver
{

    [System.Serializable]
    private class AudioClipItem
    {
        public string m_clipID = "";
        public AudioClip m_audioClip;
    }

    private Dictionary<string, AudioClip> m_stringIDToClip = new Dictionary<string, AudioClip>();

    [SerializeField]
    private List<AudioClipItem> m_audioClipItems = new List<AudioClipItem>();

    // save the dictionary to lists
    public void OnBeforeSerialize()
    {
        /*
        m_audioClipItems.Clear();
        foreach (KeyValuePair<string, AudioClip> pair in m_stringIDToClip)
        {
            AudioClipItem item = new AudioClipItem();
            item.m_clipID = pair.Key;
            item.m_audioClip = pair.Value;
            m_audioClipItems.Add(item);
        }
        */
    }

    // load dictionary from lists
    public void OnAfterDeserialize()
    {
        m_stringIDToClip.Clear();

        foreach(AudioClipItem item in m_audioClipItems)
        {
            if (!m_stringIDToClip.ContainsKey(item.m_clipID.ToUpper()))
            {
                m_stringIDToClip.Add(item.m_clipID.ToUpper(), item.m_audioClip);
            }
        }
    }

    public AudioClip GetClip(string clipID)
    {
        string upperClipID = clipID.ToUpper();
        
        if (m_stringIDToClip.ContainsKey(upperClipID))
        {
            return m_stringIDToClip[upperClipID];
        }

        return null;
    }

    public void RemoveDuplicates()
    {
        List<string> clipIDs = new List<string>();

        int index = 0;
        while (index < m_audioClipItems.Count)
        {
            AudioClipItem item = m_audioClipItems[index];
            if (clipIDs.Contains(item.m_clipID.ToUpper()))
            {
                m_audioClipItems.RemoveAt(index);
            }
            else
            {
                clipIDs.Add(item.m_clipID.ToUpper());
                index++;
            }
        }
    }
}