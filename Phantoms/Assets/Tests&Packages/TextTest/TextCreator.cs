using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct Creator
{
    public int index; 
    public string name;
    public float time;
}

public abstract class TextCreator {

    public Color32 endColor;

    public int index;

    public float duration;
    public float startTime;

    public float Progress(float time)
    {
        return (time - startTime) / duration;
    }

    public abstract void Apply(float time, int vertexIndex, Vector3[] sourceVertices, ref Vector3[] destinationVertices, ref Color32[] newVertexColors);
}

public class FadeIn:TextCreator
{
    public override void Apply(float time, int vertexIndex, Vector3[] sourceVertices, ref Vector3[] destinationVertices, ref Color32[] newVertexColors)
    {
        float progress1 = Mathf.Clamp01(Progress(time));
        Color32 prevcolor = newVertexColors[vertexIndex + 0];
        Color32 col1 = new Color32(prevcolor.r, prevcolor.g, prevcolor.b, (byte)(int)(prevcolor.a * progress1));

        Vector3 pos1 = sourceVertices[vertexIndex + 0];
        Vector3 pos3 = sourceVertices[vertexIndex + 2];
        float size = (pos1 - pos3).magnitude;
        size = size / 2.5f;

        for (int i = 0; i < 4; i++)
        {
            newVertexColors[vertexIndex + i] = col1;
            destinationVertices[vertexIndex + i].y = sourceVertices[vertexIndex + i].y + (size * (1 - progress1));
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
        Vector3 center = (sourceVertices[vertexIndex + 0] + sourceVertices[vertexIndex + 2]) / 2f;

        for (int i = 0; i < 4; i++)
        {
            destinationVertices[vertexIndex + i] = center + (sourceVertices[vertexIndex + i] - center) * progress;
        }
    }
}

public class Flip : TextCreator
{
    public override void Apply(float time, int vertexIndex, Vector3[] sourceVertices, ref Vector3[] destinationVertices, ref Color32[] newVertexColors)
    {
        float progress = Mathf.Clamp01(Progress(time));

        Vector3 center = (sourceVertices[vertexIndex + 0] + sourceVertices[vertexIndex + 2]) / 2f;

        for (int i = 0; i < 4; i++)
        {
            destinationVertices[vertexIndex + i].x = center.x + (sourceVertices[vertexIndex + i].x - center.x) * progress;
        }
    }
}
