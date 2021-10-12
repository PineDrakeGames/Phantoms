using UnityEngine;
using UnityEngine.UI;

public class GraphicPhantomTypeColorAssigner : PhantomTypeColorAssigner
{
    protected override void AssignColor(Color color)
    {
        GetComponent<Graphic>().color = color;
    }
}
