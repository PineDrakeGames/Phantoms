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

    public void Apply(float time, int vertexIndex, Vector3[] sourceVertices, ref Vector3[] destinationVertices, ref Color32[] newVertexColors)
    {
        float currentTime = (time - startTime);
        if (Effect != null)
        {
            Effect.ApplyEffectCreator(currentTime, vertexIndex, sourceVertices, ref destinationVertices, ref newVertexColors);
        }
    }
}

/*
public class FadeIn:TextCreator
{
    public override void Apply(float time, int vertexIndex, Vector3[] sourceVertices, ref Vector3[] destinationVertices, ref Color32[] newVertexColors)
    {
        float progress1 = Mathf.Clamp01(Progress(time));
        Color32 prevcolor = newVertexColors[vertexIndex + 0];
        Color32 col1 = new Color32(prevcolor.r, prevcolor.g, prevcolor.b, (byte)(int)(prevcolor.a * progress1));

        Vector3 pos1 = destinationVertices[vertexIndex + 0];
        Vector3 pos3 = destinationVertices[vertexIndex + 2];
        float size = (pos1 - pos3).magnitude;
        size = size / 2.5f;

        for (int i = 0; i < 4; i++)
        {
            newVertexColors[vertexIndex + i] = col1;
            destinationVertices[vertexIndex + i].y = destinationVertices[vertexIndex + i].y + (size * (1 - progress1));
        }
    }
}
public class Pop : TextCreator
{
    public override void Apply(float time, int vertexIndex, Vector3[] sourceVertices, ref Vector3[] destinationVertices, ref Color32[] newVertexColors)
    {
        float progress = Mathf.Clamp01(Progress(time));
        if (progress < 0.8f)
        {
            progress = (progress / 0.8f) * 1.2f;
        } else
        {
            progress = 1.2f - ((progress - 0.8f) * 5f * 0.2f);
        }
        Vector3 center = (destinationVertices[vertexIndex + 0] + destinationVertices[vertexIndex + 2]) / 2f;

        for (int i = 0; i < 4; i++)
        {
            destinationVertices[vertexIndex + i] = center + (destinationVertices[vertexIndex + i] - center) * progress;
        }
    }
}

public class Flip : TextCreator
{
    public override void Apply(float time, int vertexIndex, Vector3[] sourceVertices, ref Vector3[] destinationVertices, ref Color32[] newVertexColors)
    {
        float progress = Mathf.Clamp01(Progress(time));

        Vector3 center = (destinationVertices[vertexIndex + 0] + destinationVertices[vertexIndex + 2]) / 2f;

        for (int i = 0; i < 4; i++)
        {
            destinationVertices[vertexIndex + i].x = center.x + (destinationVertices[vertexIndex + i].x - center.x) * progress;
        }
    }
}
*/
