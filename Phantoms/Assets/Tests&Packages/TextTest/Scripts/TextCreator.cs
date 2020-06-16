using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct Creator
{
    public int index;
    public FancyTextEffect CreateType;
}

public class TextCreator
{

    public int index;
    public float startTime;

    public FancyTextEffect Effect;

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

    public void Apply(int vertexIndex, Vector3[] sourceVertices, ref Vector3[] destinationVertices, ref Color32[] newVertexColors)
    {
        float currentTime = (Time.time - startTime);
        if (Effect != null)
        {
            Effect.ApplyEffectCreator(currentTime, vertexIndex, sourceVertices, ref destinationVertices, ref newVertexColors);
        }
    }
}