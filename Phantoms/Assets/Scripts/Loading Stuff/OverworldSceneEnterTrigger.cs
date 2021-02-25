using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldSceneEnterTrigger : MonoBehaviour
{
    public string LocationID = null;

    [SerializeField]
    [HideInInspector]
    private string m_storedLocationID = null;

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Automatically add the location ID to the Overwold Scene Table, whenever a new one is made or changed.
        if (m_storedLocationID != LocationID)
        {
            // Get a list of all the other locations in this scene, to deal with edge case where 2 triggers share a name
            //  (Probably after duplicating one entrance, and then changing the ID after)
            OverworldSceneEnterTrigger[] sceneEnterTriggers = FindObjectsOfType<OverworldSceneEnterTrigger>();
            List<string> otherLocations = new List<string>();
            foreach(OverworldSceneEnterTrigger enterTrigger in sceneEnterTriggers)
            {
                if (enterTrigger != this && !string.IsNullOrEmpty(enterTrigger.LocationID))
                {
                    otherLocations.Add(enterTrigger.LocationID);
                }
            }

            // Get the current scene, and the table itself.
            OverworldSceneTable table = Resources.Load<OverworldSceneTable>("Overworld Scene Table");
            string currentScene = UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene().path;

            // If the previous location ID was not empty, and there's not another location with the same name, safely remove it from the table.
            if (!string.IsNullOrEmpty(m_storedLocationID) && !otherLocations.Contains(m_storedLocationID))
            {
                if (table.SceneToLocations.ContainsKey(currentScene))
                {
                    table.SceneToLocations[currentScene].Remove(m_storedLocationID);
                }
            }

            // If the new location ID is not empty, check if we need to add a dictionary entry, and add it in.
            if (!string.IsNullOrEmpty(LocationID))
            {
                if (!table.SceneToLocations.ContainsKey(currentScene))
                {
                    table.SceneToLocations[currentScene] = new List<string>();
                }
                // Don't add a duplicate
                if (!otherLocations.Contains(LocationID))
                {
                    table.SceneToLocations[currentScene].Add(string.Copy(LocationID));
                }
            }

            m_storedLocationID = string.Copy(LocationID);
            UnityEditor.EditorUtility.SetDirty(table);
        }
    }
#endif
}
