using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TextEffect {

    public int index;
    public float strength;

    public abstract void Apply(float time, ref UIVertex uiVertex1, ref UIVertex uiVertex2, ref UIVertex uiVertex3, ref UIVertex uiVertex4);
}

public class Wavy : TextEffect
{
    public override void Apply(float time, ref UIVertex uiVertex1, ref UIVertex uiVertex2, ref UIVertex uiVertex3, ref UIVertex uiVertex4)
    {
        Vector3 pos1 = uiVertex1.position;
        Vector3 pos3 = uiVertex3.position;
        float size = (pos1 - pos3).magnitude;
        size = (size / 6f) * strength;
        uiVertex1.position.y += Mathf.Sin(5f * time + index / 5f) * size;
        uiVertex2.position.y += Mathf.Sin(5f * time + index / 5f) * size;
        uiVertex3.position.y += Mathf.Sin(5f * time + index / 5f) * size;
        uiVertex4.position.y += Mathf.Sin(5f * time + index / 5f) * size;
    }
}

public class Jitter : TextEffect
{
    public override void Apply(float time, ref UIVertex uiVertex1, ref UIVertex uiVertex2, ref UIVertex uiVertex3, ref UIVertex uiVertex4)
    {
        Vector3 pos1 = uiVertex1.position;
        Vector3 pos3 = uiVertex3.position;
        float size = (pos1 - pos3).magnitude;
        size = (size / 14f) * strength;
        float randx = Random.Range(-1, 1) * size;
        float randy = Random.Range(-1, 1) * size;
        uiVertex1.position.y += randx;
        uiVertex1.position.x += randy;
        uiVertex2.position.y += randx;
        uiVertex2.position.x += randy;
        uiVertex3.position.y += randx;
        uiVertex3.position.x += randy;
        uiVertex4.position.y += randx;
        uiVertex4.position.x += randy;
    }
}

public class Pulse : TextEffect
{
    public override void Apply(float time, ref UIVertex uiVertex1, ref UIVertex uiVertex2, ref UIVertex uiVertex3, ref UIVertex uiVertex4)
    {
        Vector3 center = (uiVertex4.position + uiVertex3.position) / 2f;
        float stretch = Mathf.Sin(5f * time + index / 5f) * 0.1f * strength;
        Vector3 dir1 = uiVertex1.position - center;
        uiVertex1.position += dir1 * stretch;
        Vector3 dir2 = uiVertex2.position - center;
        uiVertex2.position += dir2 * stretch;
        Vector3 dir3 = uiVertex3.position - center;
        uiVertex3.position += dir3 * stretch;
        Vector3 dir4 = uiVertex4.position - center;
        uiVertex4.position += dir4 * stretch;
    }
}

public class Swivel : TextEffect
{
    public override void Apply(float time, ref UIVertex uiVertex1, ref UIVertex uiVertex2, ref UIVertex uiVertex3, ref UIVertex uiVertex4)
    {
        Vector3 center = (uiVertex1.position + uiVertex2.position + uiVertex3.position + uiVertex4.position) / 4f;
        float rotation = (Mathf.Sin(5f * time + index / 5f) * .3f * strength - Mathf.PI/2f);
        Vector3 new1, new2, new3, new4;

        Vector3 dir1 = uiVertex1.position - center;
        float mag1 = dir1.magnitude;
        float ang1 = Mathf.Atan(dir1.y / dir1.x) + rotation + Mathf.PI;
        new1 = new Vector3(mag1 * Mathf.Sin(ang1), mag1 * Mathf.Cos(ang1), 0f);

        Vector3 dir2 = uiVertex2.position - center;
        float mag2 = dir2.magnitude;
        float ang2 = Mathf.Atan(dir2.y / dir2.x) + rotation;
        new2 = new Vector3(mag2 * Mathf.Sin(ang2), mag2 * Mathf.Cos(ang2), 0f);

        Vector3 dir3 = uiVertex3.position - center;
        float mag3 = dir3.magnitude;
        float ang3 = Mathf.Atan(dir3.y / dir3.x) + rotation;
        new3 = new Vector3(mag3 * Mathf.Sin(ang3), mag3 * Mathf.Cos(ang3), 0f);

        Vector3 dir4 = uiVertex4.position - center;
        float mag4 = dir4.magnitude;
        float ang4 = Mathf.Atan(dir4.y / dir4.x) + rotation + Mathf.PI;
        new4 = new Vector3(mag4 * Mathf.Sin(ang4), mag4 * Mathf.Cos(ang4), 0f);

        uiVertex1.position = center + new2;
        uiVertex2.position = center + new1;
        uiVertex3.position = center + new4;
        uiVertex4.position = center + new3;
    }
}

public class Rainbow : TextEffect
{
    public override void Apply(float time, ref UIVertex uiVertex1, ref UIVertex uiVertex2, ref UIVertex uiVertex3, ref UIVertex uiVertex4)
    {
        float temp = ((time + (index / 3f)) * strength) % 6f;

        int r, g, b;

        // set the r
        if (temp < 1f || temp > 5f)
        {
            r = 255;
        } else if (temp > 2f && temp < 4f)
        {
            r = 0;
        } else if (temp >= 1f && temp <= 2f)
        {
            float prog = 1f - (temp - 1f);
            r = (int)(255 * prog);
        } else
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
        uiVertex1.color = col;
        uiVertex2.color = col;
        uiVertex3.color = col;
        uiVertex4.color = col;
    }
}
