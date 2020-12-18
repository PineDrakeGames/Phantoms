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
        switch (type)
        {
            case AbilityMinigameType.ANGER:
                try
                {
                    JsonUtility.FromJson<ButtonMashMinigameData>(MinigameData);
                }
                catch
                {
                    MinigameData = JsonUtility.ToJson(new ButtonMashMinigameData(), true);
                }
                break;
            case AbilityMinigameType.JOY_METER:
                try
                {
                    JsonUtility.FromJson<TimingMeterMinigameData>(MinigameData);
                }
                catch
                {
                    MinigameData = JsonUtility.ToJson(new TimingMeterMinigameData(), true);
                }
                break;
            default:
                MinigameData = "Not yet Implemented!";
                break;
        }

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