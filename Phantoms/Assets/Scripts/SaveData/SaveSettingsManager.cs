using System.Collections;
using UnityEngine;
using System.Collections.Generic;
#if !(UNITY_WEBGL || UNITY_WSA)
using System.IO;
#endif

public static class SaveSettingsManager
{
    private const string SETTINGS_FILE_NAME = "/settings.dat";

    public static void Load()
    {
        string path = Application.persistentDataPath + SETTINGS_FILE_NAME;
        string jsonData;
        try
        {
            using (StreamReader streamReader = new StreamReader(path))
            {
                jsonData = streamReader.ReadToEnd();
            }
        }
        catch (System.Exception)
        {
            Debug.Log("Save System: Error reading file: " + path);
            jsonData = string.Empty;
            Save();
        }

        DataManager.Instance.Settings = JsonUtility.FromJson<SettingsData>(jsonData);
        Debug.Log(DataManager.Instance.Settings.MasterVolume + ", " + DataManager.Instance.Settings.MusicVolume + ", " + DataManager.Instance.Settings.SoundsVolume + ", " + DataManager.Instance.Settings.LastSaveSlotPlayed);
        Debug.Log(path);
    }

    public static void Save()
    {
        string jsonData = JsonUtility.ToJson(DataManager.Instance.Settings, false);

        string path = Application.persistentDataPath + SETTINGS_FILE_NAME;

        try
        {
            using (StreamWriter streamWriter = new StreamWriter(path))
            {
                streamWriter.WriteLine(jsonData);
            }
        }
        catch (System.Exception)
        {
            Debug.LogError("Save System: Can't create file: " + path);
        }

    }
}
