using UnityEngine;
using TMPro;

struct Creator
{
    public int index;
    public FancyTextEffect CreateType;
}

public class TextCreator
{
    public int Index;
    public float startTime;
    public FancyTextEffect Effect;
    public CharacterData Data;

    public float Progress(float time)
    {
        if (Effect != null)
        {
            return (time - startTime) / Effect.MaxEffectTime;
        }
        else
        {
            return 1f;
        }
    }

    public void Apply()
    {
        float currentTime = (Time.time - startTime);
        if (Effect != null)
        {
            Data.UpdateData();
            Effect.ApplyEffectCreator(currentTime, Data);
        }
    }
}