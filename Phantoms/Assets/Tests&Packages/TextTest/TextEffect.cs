using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TextEffect
{

    public int index;
    public float strength;

    public abstract void Apply(float time, int vertexIndex, Vector3[] sourceVertices, ref Vector3[] destinationVertices, ref Color32[] newVertexColors);
}

public class Wavy : TextEffect
{
    public override void Apply(float time, int vertexIndex, Vector3[] sourceVertices, ref Vector3[] destinationVertices, ref Color32[] newVertexColors)
    {
        Vector3 pos1 = sourceVertices[vertexIndex + 0];
        Vector3 pos3 = sourceVertices[vertexIndex + 2];
        float size = (pos1 - pos3).magnitude;
        size = (size / 6f) * strength;
        for (int i = 0; i < 4; i++)
        {
            destinationVertices[vertexIndex + i].y += Mathf.Sin(5f * time + index / 5f) * size;
        }
    }
}

public class Jitter : TextEffect
{
    public override void Apply(float time, int vertexIndex, Vector3[] sourceVertices, ref Vector3[] destinationVertices, ref Color32[] newVertexColors)
    {
        Vector3 pos1 = sourceVertices[vertexIndex + 0];
        Vector3 pos3 = sourceVertices[vertexIndex + 2];
        float size = (pos1 - pos3).magnitude;
        size = (size / 14f) * strength;
        float randx = Random.Range(-1, 1) * size;
        float randy = Random.Range(-1, 1) * size;
        for (int i = 0; i < 4; i++)
        {
            destinationVertices[vertexIndex + i].x += randx;
            destinationVertices[vertexIndex + i].y += randy;
        }
    }
}

public class Pulse : TextEffect
{
    public override void Apply(float time, int vertexIndex, Vector3[] sourceVertices, ref Vector3[] destinationVertices, ref Color32[] newVertexColors)
    {
        Vector3 center = (sourceVertices[vertexIndex + 0] + sourceVertices[vertexIndex + 2]) / 2f;
        float stretch = Mathf.Sin(5f * time + index / 5f) * 0.1f * strength;
        for (int i = 0; i < 4; i++)
        {
            destinationVertices[vertexIndex + i] = (sourceVertices[vertexIndex + i] - center) * stretch;
        }
    }
}

public class Swivel : TextEffect
{
    public override void Apply(float time, int vertexIndex, Vector3[] sourceVertices, ref Vector3[] destinationVertices, ref Color32[] newVertexColors)
    {
        Vector3 center = (sourceVertices[vertexIndex + 0] + sourceVertices[vertexIndex + 2]) / 2f;
        float rotation = (Mathf.Sin(5f * time + index / 5f) * .3f * strength) * Mathf.Rad2Deg;
        for (int i = 0; i < 4; i++)
        {
            Vector3 dir = sourceVertices[vertexIndex + i] - center;
            destinationVertices[vertexIndex + i] = center + (Quaternion.Euler(0f, 0f, rotation) * dir);
        }
    }
}

public class Rainbow : TextEffect
{
    public override void Apply(float time, int vertexIndex, Vector3[] sourceVertices, ref Vector3[] destinationVertices, ref Color32[] newVertexColors)
    {
        float temp = ((time + (index / 3f)) * strength) % 6f;

        int r, g, b;

        // set the r
        if (temp < 1f || temp > 5f)
        {
            r = 255;
        }
        else if (temp > 2f && temp < 4f)
        {
            r = 0;
        }
        else if (temp >= 1f && temp <= 2f)
        {
            float prog = 1f - (temp - 1f);
            r = (int)(255 * prog);
        }
        else
        {
            float prog = temp - 4f;
            r = (int)(255 * prog);
        }

        // set the g
        if (temp > 1f && temp < 3f)
        {
            g = 255;
        }
        else if (temp > 4f)
        {
            g = 0;
        }
        else if (temp <= 1f)
        {
            float prog = temp;
            g = (int)(255 * prog);
        }
        else
        {
            float prog = 1f - (temp - 3f);
            g = (int)(255 * prog);
        }

        // set the b
        if (temp > 3f && temp < 5f)
        {
            b = 255;
        }
        else if (temp < 2f)
        {
            b = 0;
        }
        else if (temp >= 2f && temp <= 3f)
        {
            float prog = temp - 1f;
            b = (int)(255 * prog);
        }
        else
        {
            float prog = 1f - (temp - 5f);
            b = (int)(255 * prog);
        }

        Color32 col = new Color32((byte)r, (byte)g, (byte)b, (byte)255);
        for (int i = 0; i < 4; i++)
        {
            newVertexColors[vertexIndex + i] = col;
        }
    }
}
