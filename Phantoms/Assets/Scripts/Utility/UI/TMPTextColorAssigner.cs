using UnityEngine;
using TMPro;

public class TMPTextColorAssigner : ColorAssigner
{
    protected override void AssignColor(Color color)
    {
        GetComponent<TMP_Text>().color = color;
    }
}