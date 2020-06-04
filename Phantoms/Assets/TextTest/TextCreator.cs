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

    public abstract void Apply(float time, ref UIVertex uiVertex1, ref UIVertex uiVertex2, ref UIVertex uiVertex3, ref UIVertex uiVertex4);
}

public class FadeIn:TextCreator
{
    public override void Apply(float time, ref UIVertex uiVertex1, ref UIVertex uiVertex2, ref UIVertex uiVertex3, ref UIVertex uiVertex4)
    {
        float progress1 = Mathf.Clamp01(Progress(time));
        Color32 prevcolor = uiVertex1.color;
        Color32 col1 = new Color32(prevcolor.r, prevcolor.g, prevcolor.b, (byte)(int)(prevcolor.a * progress1));

        uiVertex1.color = col1;
        uiVertex2.color = col1;
        uiVertex3.color = col1;
        uiVertex4.color = col1;

        Vector3 pos1 = uiVertex1.position;
        Vector3 pos3 = uiVertex3.position;
        float size = (pos1 - pos3).magnitude;
        size = size / 2.5f;
        uiVertex1.position.y += size * (1 - progress1);
        uiVertex2.position.y += size * (1 - progress1);
        uiVertex3.position.y += size * (1 - progress1);
        uiVertex4.position.y += size * (1 - progress1);
    }
}
public class Pop : TextCreator
{
    public override void Apply(float time, ref UIVertex uiVertex1, ref UIVertex uiVertex2, ref UIVertex uiVertex3, ref UIVertex uiVertex4)
    {
        float progress = Mathf.Clamp01(Progress(time));
        if (progress < 0.8f)
        {
            progress = (progress / 0.8f) * 1.2f;
        } else
        {
            progress = 1.2f - ((progress - 0.8f) * 5f * 0.2f);
        }
        Vector3 center = (uiVertex4.position + uiVertex3.position) / 2f;
        Vector3 dir1 = uiVertex1.position - center;
        uiVertex1.position = center + dir1 * progress;
        Vector3 dir2 = uiVertex2.position - center;
        uiVertex2.position = center + dir2 * progress;
        Vector3 dir3 = uiVertex3.position - center;
        uiVertex3.position = center + dir3 * progress;
        Vector3 dir4 = uiVertex4.position - center;
        uiVertex4.position = center + dir4 * progress;
    }
}

public class Flip : TextCreator
{
    public override void Apply(float time, ref UIVertex uiVertex1, ref UIVertex uiVertex2, ref UIVertex uiVertex3, ref UIVertex uiVertex4)
    {
        float progress = Mathf.Clamp01(Progress(time));
        Vector3 center1 = (uiVertex1.position + uiVertex2.position) / 2f;
        Vector3 center2 = (uiVertex4.position + uiVertex3.position) / 2f;
        Vector3 dir1 = uiVertex1.position - center1;
        uiVertex1.position = center1 + dir1 * progress;
        Vector3 dir2 = uiVertex2.position - center1;
        uiVertex2.position = center1 + dir2 * progress;
        Vector3 dir3 = uiVertex3.position - center2;
        uiVertex3.position = center2 + dir3 * progress;
        Vector3 dir4 = uiVertex4.position - center2;
        uiVertex4.position = center2 + dir4 * progress;
    }
}
