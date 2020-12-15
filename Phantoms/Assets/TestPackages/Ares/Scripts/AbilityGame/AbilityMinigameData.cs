using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum AbilityMinigameType
{
    LOVE,
    FEAR,
    ANGER,
    SADNESS,
    CALM,
    HOPE,
    JOY_LIGHTS,
    JOY_METER,
    SHAME
}

[CreateAssetMenu(menuName = "Phantoms/Minigame Data")]
public class AbilityMinigameData : ScriptableObject
{
    public AbilityMinigameType type;

    [TextArea(10, 40)]
    public string MinigameData = string.Empty;

    public void UpdateData()
    {
        MinigameData = JsonUtility.ToJson(new TimingMeterMinigameData(), true);
    }
}

[CustomEditor(typeof(AbilityMinigameData))]
public class AbilityMinigameDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var script = (AbilityMinigameData)target;

        if (GUILayout.Button("Reset Data", GUILayout.Height(40)))
        {
            script.UpdateData();
        }

    }
}