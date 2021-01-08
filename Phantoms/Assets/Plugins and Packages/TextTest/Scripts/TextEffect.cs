using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextEffect
{
    public int Index;
    public FancyTextEffect Effect;
    public CharacterData Data;

    public void Apply()
    {
        Data.UpdateData();
        Effect.ApplyEffectConstant(Data);
    }
}