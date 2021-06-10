using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class SaveDataManager
{
    // TODO: Actually save stuff! For now, just storing it statically.
    private static Dictionary<string, bool> m_flags = new Dictionary<string, bool>();

    public static UnityEvent<string, bool> OnFlagUpdate = new UnityEvent<string, bool>();
    
    public static bool CheckFlag(string flagName)
    {
        if (string.IsNullOrEmpty(flagName))
        {
            return false;
        }

        if (m_flags.ContainsKey(flagName) && m_flags[flagName])
        {
            return true;
        }
        return false;
    }

    public static void SetFlag(string flagName, bool flagValue = true)
    {
        if (string.IsNullOrEmpty(flagName))
        {
            return;
        }

        if (m_flags.ContainsKey(flagName))
        {
            m_flags[flagName] = flagValue;
        }
        else
        {
            m_flags.Add(flagName, flagValue);
        }

        OnFlagUpdate.Invoke(flagName, flagValue);
    }
}
