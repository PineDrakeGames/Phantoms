using UnityEngine;
using UnityEngine.UI;

public class GraphicColorAssigner : ColorAssigner
{
    protected override void AssignColor(Color color)
    {
        GetComponent<Graphic>().color = color;
    }
}