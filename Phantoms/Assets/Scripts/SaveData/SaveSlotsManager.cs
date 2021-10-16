using System.Collections;
using UnityEngine;
using UnityEngine;
using System.Collections.Generic;
#if !(UNITY_WEBGL || UNITY_WSA)
using System.IO;
#endif

// Should just contain the information needed to display what's up with each slot.
// For now, just number and last time saved - but eventually want stuff like current chapter
// (Just something to indicate which save this is)
[System.Serializable]
public class SaveSlot
{
    public int SlotNumber = 0;
    public string LastSave = null;

    public SaveSlot(int slotNumber)
    {
        SlotNumber = slotNumber;
        LastSave = System.DateTime.Now.ToString("MM/dd/yyyy, hh:mm tt");
    }
}

[System.Serializable]
public class SaveSlots
{
    public List<SaveSlot> Slots = new List<SaveSlot>();
}


public static class SaveSlotManager
{

    public static SaveSlots SavedData = new SaveSlots();

    private const string SLOTS_FILE_NAME = "/saveslots.dat";

    public const int NUM_SLOTS = 3;

    public static void Load()
    {
        string path = Application.persistentDataPath + SLOTS_FILE_NAME;
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
        }

        if (Equals(SavedData, default(SaveSlots)))
        {
            SavedData = JsonUtility.FromJson<SaveSlots>(jsonData);
        }
        else
        {
            JsonUtility.FromJsonOverwrite(jsonData, SavedData);
        }
    }

    public static void Save()
    {
        UpdateSlots();
        
        string jsonData = JsonUtility.ToJson(SavedData, true);

        string path = Application.persistentDataPath + SLOTS_FILE_NAME;

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

    public static void UpdateSlots()
    {
        if (SaveDataManager.CurrentSaveSlot < 0 || SaveDataManager.CurrentSaveSlot > NUM_SLOTS)
        {
            return;
        }

        bool foundSlot = false;
        foreach(SaveSlot slot in SavedData.Slots)
        {
            if (slot.SlotNumber == SaveDataManager.CurrentSaveSlot)
            {
                slot.LastSave = System.DateTime.Now.ToString("MM/dd/yyyy, hh:mm tt");
                foundSlot = true;
            }
        }

        if (!foundSlot)
        {
            SaveSlot slot = new SaveSlot(SaveDataManager.CurrentSaveSlot);
            slot.SlotNumber = SaveDataManager.CurrentSaveSlot;
            slot.LastSave = System.DateTime.Now.ToString("MM/dd/yyyy, hh:mm tt");
            SavedData.Slots.Add(slot);
        }
    }

    public static SaveSlot GetSlotData(int slotNumber)
    {
        for (int i = 0; i < SavedData.Slots.Count; i++)
        {
            SaveSlot slot = SavedData.Slots[i];
            if (slot.SlotNumber == slotNumber)
            {
                return slot;
            }
        }
        return null;
    }

    public static void CleanUpSlots()
    {
        for (int i = 0; i < SavedData.Slots.Count; i++)
        {
            SaveSlot slot = SavedData.Slots[i];
            if (!PixelCrushers.SaveSystem.HasSavedGameInSlot(slot.SlotNumber))
            {
                SavedData.Slots.RemoveAt(i);
                i--;
            }
        }
    }
}
