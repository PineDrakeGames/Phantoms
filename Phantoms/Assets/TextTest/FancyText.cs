using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FancyText : BaseMeshEffect {


    // Bools that say if we are active and if we are currently revealing stuff
    [HideInInspector]
    public bool active = false;
    [HideInInspector]
    public bool revealing = false;

    ////  These are things the script needs  ////

    //Reveal text strings and things 
    Text textComponent;
    string textInputString;
    string textOutputString;
    int charIndex = 0;
    public float charDelay = 0.05f;
    Color32 startColor;
    float progress;

    //things related to text display speed and pauses
    List<SpeedOption> speeds = new List<SpeedOption>();
    int numSpeeds = 0;
    public float newLinePause = 0.8f;

    //Text Effects stuff
    List<TextEffect> effects = new List<TextEffect>();
    string[] seperator = { ">>" };
    public enum EffectType
    {
        NONE,
        WAVY,
        JITTER,
        PULSE,
        SWIVEL,
        RAINBOW
    };
    public EffectType effectType = EffectType.NONE;
    public float effectStrength = 1f;

    //Text Creation stuff
    List<TextCreator> creators = new List<TextCreator>();
    List<Creator> creatorIndexes = new List<Creator>();
    public enum CreateType
    {
        INSTANT,
        FADEIN,
        POP,
        FLIP
    };
    public CreateType createtype = CreateType.INSTANT;
    public float createTime = 0.5f;
    float numCreators = 0;

    // Audio Stuff
    AudioSource audio;
    bool hasAudio = false;

    protected override void Start()
    {
        textComponent = GetComponent<Text>();
        if (GetComponent<AudioSource>() != null)
        {
            audio = GetComponent<AudioSource>();
            hasAudio = true;
        }
    }

    // Update is called once per frame
    void Update () {
        //tell the mesh that the verts must be redrawn
        if (active)
        {
            graphic.SetAllDirty();
        }
    }

    public override void ModifyMesh(VertexHelper vh)
    {
        if (!active)
        {
            return;
        }

        // get the amount of verts
        int count = vh.currentVertCount;

        // Apply the effects to letters first, in case they are overridden by later changes
        foreach(TextEffect effect in effects)
        {
            // make sure that the letter is actually displayed
            if (effect.index * 4 < count)
            {
                UIVertex uiVertex1 = new UIVertex();
                UIVertex uiVertex2 = new UIVertex();
                UIVertex uiVertex3 = new UIVertex();
                UIVertex uiVertex4 = new UIVertex();
                vh.PopulateUIVertex(ref uiVertex1, effect.index * 4 + 0);
                vh.PopulateUIVertex(ref uiVertex2, effect.index * 4 + 1);
                vh.PopulateUIVertex(ref uiVertex3, effect.index * 4 + 2);
                vh.PopulateUIVertex(ref uiVertex4, effect.index * 4 + 3);
                effect.Apply(Time.time, ref uiVertex1, ref uiVertex2, ref uiVertex3, ref uiVertex4);
                vh.SetUIVertex(uiVertex1, effect.index * 4 + 0);
                vh.SetUIVertex(uiVertex2, effect.index * 4 + 1);
                vh.SetUIVertex(uiVertex3, effect.index * 4 + 2);
                vh.SetUIVertex(uiVertex4, effect.index * 4 + 3);
            }
        }

        // Next we do the animations for characters that are currently being put in
        foreach (TextCreator creator in creators)
        {
            if (creator.index * 4 < count)
            {
                UIVertex uiVertex1 = new UIVertex();
                UIVertex uiVertex2 = new UIVertex();
                UIVertex uiVertex3 = new UIVertex();
                UIVertex uiVertex4 = new UIVertex();
                vh.PopulateUIVertex(ref uiVertex1, creator.index * 4 + 0);
                vh.PopulateUIVertex(ref uiVertex2, creator.index * 4 + 1);
                vh.PopulateUIVertex(ref uiVertex3, creator.index * 4 + 2);
                vh.PopulateUIVertex(ref uiVertex4, creator.index * 4 + 3);
                creator.Apply(Time.time, ref uiVertex1, ref uiVertex2, ref uiVertex3, ref uiVertex4);
                vh.SetUIVertex(uiVertex1, creator.index * 4 + 0);
                vh.SetUIVertex(uiVertex2, creator.index * 4 + 1);
                vh.SetUIVertex(uiVertex3, creator.index * 4 + 2);
                vh.SetUIVertex(uiVertex4, creator.index * 4 + 3);
            }
        }
        // destroy any creators we no longer need to do
        while (true)
        {
            bool done = true;
            foreach(TextCreator creator in creators)
            {
                if(creator.Progress(Time.time) >= 1f)
                {
                    done = false;
                    creators.Remove(creator);
                    break;
                }
            }
            if (done)
            {
                break;
            }
        }

        //finally, set everything that hasn't appeared yet to an invisible color
        for (int i = charIndex * 4; i < count; i += 4)
        {
            for (int j = 0; j < 4; j++)
            {
                UIVertex uiVertex = new UIVertex();
                vh.PopulateUIVertex(ref uiVertex, i + j);
                uiVertex.color = new Color32((byte)0, (byte)0, (byte)0, 0);
                vh.SetUIVertex(uiVertex, i + j);
            }
        }

    }

    private void ParseText()
    {
        textOutputString = "";
        for (int i = 0; i < textInputString.Length; i++)
        {
            if (textInputString[i] == '[')
            {
                i += 1;
                string option = "";
                while (textInputString[i] != ']')
                {
                    if (textInputString[i] == '=')
                    {
                        i += 1;
                    }
                    else
                    {
                        option += textInputString[i];
                        i += 1;
                    }
                }
                if (option == "[")
                {
                    textOutputString += '[';
                }
                else if ((option.Substring(0, 3)).ToLower() == "pop")
                {
                    Creator temp = new Creator();
                    temp.index = textOutputString.Length;
                    temp.name = "pop";
                    if (option.Length > 3)
                    {
                        temp.time = float.Parse(option.Substring(3));
                    } else
                    {
                        temp.time = -1;
                    }
                    numCreators += 1;
                    creatorIndexes.Add(temp);
                }
                else if ((option.Substring(0, 4)).ToLower() == "none")
                {
                    string word = option.Substring(4);
                    textOutputString += word;
                }
                else if ((option.Substring(0, 4)).ToLower() == "flip")
                {
                    Creator temp = new Creator();
                    temp.index = textOutputString.Length;
                    temp.name = "flip";
                    if (option.Length > 3)
                    {
                        temp.time = float.Parse(option.Substring(4));
                    }
                    else
                    {
                        temp.time = -1;
                    }
                    numCreators += 1;
                    creatorIndexes.Add(temp);
                }
                else if ((option.Substring(0, 4)).ToLower() == "wavy")
                {
                    string input = option.Substring(4);
                    string[] splitString = input.Split(seperator, System.StringSplitOptions.RemoveEmptyEntries);
                    string word = splitString[0];
                    foreach (char letter in word)
                    {
                        TextEffect effect = new Wavy();
                        effect.index = textOutputString.Length;
                        if (splitString.Length == 2)
                        {
                            effect.strength = float.Parse(splitString[1]);
                        }
                        else
                        {
                            effect.strength = 1f;
                        }
                        effects.Add(effect);
                        textOutputString += letter;
                    }
                }
                
                else if ((option.Substring(0, 5)).ToLower() == "pulse")
                {
                    string input = option.Substring(5);
                    string[] splitString = input.Split(seperator, System.StringSplitOptions.RemoveEmptyEntries);
                    string word = splitString[0];
                    foreach (char letter in word)
                    {
                        TextEffect effect = new Pulse();
                        effect.index = textOutputString.Length;
                        if (splitString.Length == 2)
                        {
                            effect.strength = float.Parse(splitString[1]);
                        }
                        else
                        {
                            effect.strength = 1f;
                        }
                        effects.Add(effect);
                        textOutputString += letter;
                    }
                }
                
                else if ((option.Substring(0, 5)).ToLower() == "speed")
                {
                    SpeedOption temp = new SpeedOption();
                    temp.type = "speed";
                    temp.index = textOutputString.Length;
                    temp.speedNewTime = float.Parse(option.Substring(5));
                    speeds.Add(temp);
                    numSpeeds += 1;
                }
                else if ((option.Substring(0, 5)).ToLower() == "pause")
                {
                    SpeedOption temp = new SpeedOption();
                    temp.type = "pause";
                    temp.index = textOutputString.Length;
                    temp.pauseTime = float.Parse(option.Substring(5));
                    speeds.Add(temp);
                    numSpeeds += 1;
                }
                else if ((option.Substring(0, 6)).ToLower() == "swivel")
                {
                    string input = option.Substring(6);
                    string[] splitString = input.Split(seperator, System.StringSplitOptions.RemoveEmptyEntries);
                    string word = splitString[0];
                    foreach (char letter in word)
                    {
                        TextEffect effect = new Swivel();
                        effect.index = textOutputString.Length;
                        if (splitString.Length == 2)
                        {
                            effect.strength = float.Parse(splitString[1]);
                        }
                        else
                        {
                            effect.strength = 1f;
                        }
                        effects.Add(effect);
                        textOutputString += letter;
                    }
                }
                else if ((option.Substring(0, 6)).ToLower() == "jitter")
                {
                    string input = option.Substring(6);
                    string[] splitString = input.Split(seperator, System.StringSplitOptions.RemoveEmptyEntries);
                    string word = splitString[0];
                    foreach (char letter in word)
                    {
                        TextEffect effect = new Jitter();
                        effect.index = textOutputString.Length;
                        if (splitString.Length == 2)
                        {
                            effect.strength = float.Parse(splitString[1]);
                        } else
                        {
                            effect.strength = 1f;
                        }
                        effects.Add(effect);
                        textOutputString += letter;
                    }
                }
                else if ((option.Substring(0, 6)).ToLower() == "fadein")
                {
                    Creator temp = new Creator();
                    temp.index = textOutputString.Length;
                    temp.name = "fadein";
                    if (option.Length > 6)
                    {
                        temp.time = float.Parse(option.Substring(6));
                    }
                    else
                    {
                        temp.time = -1;
                    }
                    numCreators += 1;
                    creatorIndexes.Add(temp);
                }
                else if ((option.Substring(0, 7)).ToLower() == "rainbow")
                {
                    string input = option.Substring(7);
                    string[] splitString = input.Split(seperator, System.StringSplitOptions.RemoveEmptyEntries);
                    string word = splitString[0];
                    foreach (char letter in word)
                    {
                        TextEffect effect = new Rainbow();
                        effect.index = textOutputString.Length;
                        if (splitString.Length == 2)
                        {
                            effect.strength = float.Parse(splitString[1]);
                        }
                        else
                        {
                            effect.strength = 1f;
                        }
                        effects.Add(effect);
                        textOutputString += letter;
                    }
                }
                else if ((option.Substring(0, 7)).ToLower() == "instant")
                {
                    Creator temp = new Creator();
                    temp.index = textOutputString.Length;
                    temp.name = "instant";
                    temp.time = -1;
                    numCreators += 1;
                    creatorIndexes.Add(temp);
                }
                else if ((option.Substring(0, 7)).ToLower() == "nlpause")
                {
                    SpeedOption temp = new SpeedOption();
                    temp.type = "nlpause";
                    temp.index = textOutputString.Length;
                    temp.pauseTime = float.Parse(option.Substring(7));
                    speeds.Add(temp);
                    numSpeeds += 1;
                }
            }
            else if (textInputString[i] == '\n')
            {
                SpeedOption temp = new SpeedOption();
                temp.type = "nl";
                temp.index = textOutputString.Length;;
                speeds.Add(temp);
                numSpeeds += 1;
                textOutputString += textInputString[i];
            }
            else
            {
                TextEffect effect;
                switch (effectType)
                {
                    case EffectType.JITTER:
                        effect = new Jitter();
                        effect.index = textOutputString.Length;
                        effect.strength = effectStrength;
                        effects.Add(effect);
                        break;
                    case EffectType.WAVY:
                        effect = new Wavy();
                        effect.index = textOutputString.Length;
                        effect.strength = effectStrength;
                        effects.Add(effect);
                        break;
                    case EffectType.PULSE:
                        effect = new Pulse();
                        effect.index = textOutputString.Length;
                        effect.strength = effectStrength;
                        effects.Add(effect);
                        break;
                    case EffectType.SWIVEL:
                        effect = new Swivel();
                        effect.index = textOutputString.Length;
                        effect.strength = effectStrength;
                        effects.Add(effect);
                        break;
                    case EffectType.RAINBOW:
                        effect = new Rainbow();
                        effect.index = textOutputString.Length;
                        effect.strength = effectStrength;
                        effects.Add(effect);
                        break;
                }
                textOutputString += textInputString[i];
            }
        }
    }


    IEnumerator DisplayText()
    {
        charIndex = 0;
        int speedIndex = 0;
        int creatorsIndex = 0;
        while (charIndex < textOutputString.Length)
        {
            if (speedIndex < numSpeeds && speeds[speedIndex].index == charIndex)
            {
                if (speeds[speedIndex].type == "speed")
                {
                    charDelay = speeds[speedIndex].speedNewTime;
                }
                if (speeds[speedIndex].type == "nlpause")
                {
                    newLinePause = speeds[speedIndex].pauseTime;
                }
                if (speeds[speedIndex].type == "nl")
                {
                    yield return new WaitForSeconds(newLinePause);
                }
                if (speeds[speedIndex].type == "pause")
                {
                    yield return new WaitForSeconds(speeds[speedIndex].pauseTime);
                }
                speedIndex += 1;
            } else if (creatorsIndex < numCreators && creatorIndexes[creatorsIndex].index == charIndex)
            {
                if (creatorIndexes[creatorsIndex].name == "instant")
                {
                    createtype = CreateType.INSTANT;
                }
                if (creatorIndexes[creatorsIndex].name == "fadein")
                {
                    createtype = CreateType.FADEIN;
                    if (creatorIndexes[creatorsIndex].time != -1f)
                    {
                        createTime = creatorIndexes[creatorsIndex].time;
                    }
                }
                if (creatorIndexes[creatorsIndex].name == "pop")
                {
                    createtype = CreateType.POP;
                    if (creatorIndexes[creatorsIndex].time != -1f)
                    {
                        createTime = creatorIndexes[creatorsIndex].time;
                    }
                }
                if (creatorIndexes[creatorsIndex].name == "flip")
                {
                    createtype = CreateType.FLIP;
                    if (creatorIndexes[creatorsIndex].time != -1f)
                    {
                        createTime = creatorIndexes[creatorsIndex].time;
                    }
                }
                creatorsIndex += 1;
            }
            else
            {
                char letter = textOutputString[charIndex];
                if (letter == ' ' || letter == '\n')
                {
                    charIndex += 1;
                }
                else if (letter == '<')
                {
                    while (letter != '>')
                    {
                        charIndex += 1;
                        letter = textOutputString[charIndex];
                    }
                }

                else
                {
                    TextCreator temp;
                    switch (createtype)
                    {
                        case CreateType.INSTANT:
                            charIndex += 1;
                            break;
                        case CreateType.FADEIN:
                            temp = new FadeIn();
                            temp.index = charIndex;
                            temp.startTime = Time.time;
                            temp.duration = createTime;
                            temp.endColor = startColor;
                            creators.Add(temp);
                            charIndex += 1;
                            break;
                        case CreateType.POP:
                            temp = new Pop();
                            temp.index = charIndex;
                            temp.startTime = Time.time;
                            temp.duration = createTime;
                            temp.endColor = startColor;
                            creators.Add(temp);
                            charIndex += 1;
                            break;
                        case CreateType.FLIP:
                            temp = new Flip();
                            temp.index = charIndex;
                            temp.startTime = Time.time;
                            temp.duration = createTime;
                            temp.endColor = startColor;
                            creators.Add(temp);
                            charIndex += 1;
                            break;
                    }
                    if (charDelay != 0f)
                    {
                        if (hasAudio) { audio.Play(); }
                        yield return new WaitForSeconds(charDelay);
                    }
                }
            }
        }
        revealing = false;
    }

    public void SetText(string text)
    {
        if (!active) { active = true; }
        if (revealing) {
            StopCoroutine("DisplayText");
        }
        else { revealing = true; }

        // Resetting all variables
        if (textComponent == null)
        {
            textComponent = GetComponent<Text>();
        }
        startColor = textComponent.color;
        charIndex = 0;
        speeds.Clear();
        numSpeeds = 0;
        effects.Clear();
        creators.Clear();
        creatorIndexes.Clear();
        numCreators = 0;

        textInputString = text;
        ParseText();
        textComponent.text = textOutputString;
        startColor = textComponent.color;
        StartCoroutine("DisplayText");
    }

    public void FinishLine()
    {
        if (revealing)
        {
            StopCoroutine("DisplayText");
            numSpeeds = 0;
            numCreators = 0;
            charIndex = textOutputString.Length;
            creators.Clear();
            creatorIndexes.Clear();
            speeds.Clear();
            revealing = false;
        }
    }
}


