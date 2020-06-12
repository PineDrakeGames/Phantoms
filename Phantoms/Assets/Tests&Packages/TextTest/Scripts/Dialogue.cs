using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialogue : MonoBehaviour {


    public GameObject canvas;
    public FancyText fancyText;


    public bool active = false;

    public FancyText.CreateType createType = FancyText.CreateType.INSTANT;
    public FancyText.EffectType effectType = FancyText.EffectType.NONE;
    public float effectStrength = 1f;
    [Range(0, 1f)]
    public float charDelay = 0.05f;
    [Range(0, 3f)]
    public float createTime = 0.5f;
    public float newLinePause = 0.8f;

    public bool usingFile = false;
    public TextAsset inputFile;
    [TextArea]
    public string[] inputStrings;

    int index = 0;

	// Use this for initialization
	void Start () {
		if (usingFile)
        {
            string temp = inputFile.text;
            inputStrings = temp.Split(new string[] { "\r\n\r\n" }, System.StringSplitOptions.None);
        }
	}
	
	// Update is called once per frame
	void Update () {
		if (active && Input.GetMouseButtonDown(0))
        {
            Trigger();
        }
	}

    private void Trigger()
    {
        if (index == 0)
        {
            canvas.SetActive(true);
            SetVariables();
        }

        if (fancyText.revealing)
        {
            fancyText.FinishLine();
        }
        else if (index < inputStrings.Length) {
            fancyText.SetText(inputStrings[index]);
            index += 1;
        } else
        {
            canvas.SetActive(false);
            index = 0;
        }
    }

    private void SetVariables()
    {
        fancyText.createtype = createType;
        fancyText.createTime = createTime;
        fancyText.effectType = effectType;
        fancyText.charDelay = charDelay;
        fancyText.newLinePause = newLinePause;     
    }
}
