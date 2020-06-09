using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FancyText : MonoBehaviour
{
    /// Enums ///
    public enum EffectType
    {
        NONE,
        WAVY,
        JITTER,
        PULSE,
        SWIVEL,
        RAINBOW
    };

    public enum CreateType
    {
        INSTANT,
        FADEIN,
        POP,
        FLIP
    };

    public enum SpeedModifier
    {
        SPEED,
        PAUSE,
        NEWLINEPAUSE,
        NEWLINE
    };

    // Bools that say if we are active and if we are currently revealing stuff
    [HideInInspector]
    public bool active = false;
    [HideInInspector]
    public bool revealing = false;

    ////  These are things the script needs  ////

    //Reveal text strings and things 
    TMP_Text m_TextComponent;
    string textInputString;
    string textOutputString;
    int charIndex = 0;
    int displayedCharIndex = 0;
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
    public EffectType effectType = EffectType.NONE;
    public float effectStrength = 1f;
    //Text Creation stuff
    List<TextCreator> creators = new List<TextCreator>();
    List<Creator> creatorIndexes = new List<Creator>();

    public CreateType createtype = CreateType.INSTANT;
    public float createTime = 0.5f;
    private float numCreators = 0;

    // Audio Stuff
    AudioSource audio;
    bool hasAudio = false;

    private void Start()
    {
        m_TextComponent = GetComponent<TMP_Text>();
        if (GetComponent<AudioSource>() != null)
        {
            audio = GetComponent<AudioSource>();
            hasAudio = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //tell the mesh that the verts must be redrawn
        if (active)
        {
            ModifyMesh();
        }
    }

    private void ModifyMesh()
    {
        m_TextComponent.ForceMeshUpdate();

        TMP_TextInfo textInfo = m_TextComponent.textInfo;

        Matrix4x4 matrix;


        // Cache the vertex data of the text object as the Jitter FX is applied to the original position of the characters.
        TMP_MeshInfo[] cachedMeshInfo = textInfo.CopyMeshInfoVertexData();
        int characterCount = textInfo.characterCount;

        Color32[] newVertexColors;

        // Apply the effects to letters first, in case they are overridden by later changes
        foreach (TextEffect effect in effects)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[effect.index];

            // Skip characters that are not visible and thus have no geometry to manipulate.
            if (!charInfo.isVisible)
                continue;

            int materialIndex = textInfo.characterInfo[effect.index].materialReferenceIndex;
            int vertexIndex = textInfo.characterInfo[effect.index].vertexIndex;
            Vector3[] sourceVertices = cachedMeshInfo[materialIndex].vertices;
            Vector3[] destinationVertices = textInfo.meshInfo[materialIndex].vertices;
            newVertexColors = textInfo.meshInfo[materialIndex].colors32;

            effect.Apply(Time.time, vertexIndex, sourceVertices, ref destinationVertices, ref newVertexColors);
        }

        // Next we do the animations for characters that are currently being put in
        foreach (TextCreator creator in creators)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[creator.index];

            // Skip characters that are not visible and thus have no geometry to manipulate.
            if (!charInfo.isVisible)
                continue;

            int materialIndex = textInfo.characterInfo[creator.index].materialReferenceIndex;
            int vertexIndex = textInfo.characterInfo[creator.index].vertexIndex;
            Vector3[] sourceVertices = cachedMeshInfo[materialIndex].vertices;
            Vector3[] destinationVertices = textInfo.meshInfo[materialIndex].vertices;
            newVertexColors = textInfo.meshInfo[materialIndex].colors32;

            creator.Apply(Time.time, vertexIndex, sourceVertices, ref destinationVertices, ref newVertexColors);
        }
        // destroy any creators we no longer need to do
        while (true)
        {
            bool done = true;
            foreach (TextCreator creator in creators)
            {
                if (creator.Progress(Time.time) >= 1f)
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
        for (int i = displayedCharIndex; i < characterCount; i += 1)
        {
            int materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
            newVertexColors = textInfo.meshInfo[materialIndex].colors32;
            int vertexIndex = textInfo.characterInfo[i].vertexIndex;

            // Only change the vertex color if the text element is visible.
            if (textInfo.characterInfo[i].isVisible)
            {
                Color32 c0 = new Color32(0, 0, 0, 0);

                newVertexColors[vertexIndex + 0] = c0;
                newVertexColors[vertexIndex + 1] = c0;
                newVertexColors[vertexIndex + 2] = c0;
                newVertexColors[vertexIndex + 3] = c0;
            }
        }

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
        {
            textInfo.meshInfo[i].mesh.vertices = textInfo.meshInfo[i].vertices;
            m_TextComponent.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
        }

        m_TextComponent.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }

    private void ParseText()
    {
        textOutputString = "";
        int index = 0;
        for (int i = 0; i < textInputString.Length; i++)
        {
            // This is meant to ignore any rich text used, such as for color or font size.
            // TODO: More error checking.
            if (textInputString[i] == '<')
            {
                textOutputString += textInputString[i];
                i += 1;
                while (textInputString[i] != '>')
                {
                    textOutputString += textInputString[i];
                    i += 1;
                }
                textOutputString += textInputString[i];
                i += 1;
            }
            // If we detect a bracket, that means what follows should be a text effect option.
            if (textInputString[i] == '[')
            {
                i += 1; // skip over the bracket
                string option = "";
                string value = "";
                // iterate over string, stopping when we find an end bracket.
                // If there is an equals sign, split the strings so the first part is the option and
                // the second part is the value.
                while (textInputString[i] != ']')
                {
                    // Once we fine an =, continue iterating the same way but adding to the value instead.
                    if (textInputString[i] == '=')
                    {
                        i += 1;
                        while (textInputString[i] != ']')
                        {
                            value += textInputString[i];
                            i += 1;
                        }
                    }
                    else
                    {
                        option += textInputString[i];
                        i += 1;
                    }
                }

                option.ToLower();

                switch (option)
                {
                    case "[":
                        textOutputString += '[';
                        index += 1;
                        break;
                    // Create types
                    case "pop":
                        ParseTextCreator(CreateType.POP, index, value);
                        break;
                    case "flip":
                        ParseTextCreator(CreateType.FLIP, index, value);
                        break;
                    case "fadein":
                        ParseTextCreator(CreateType.FADEIN, index, value);
                        break;
                    case "instant":
                        ParseTextCreator(CreateType.INSTANT, index, value);
                        break;
                    // Effect types
                    case "wavy":
                        ParseTextEffect(EffectType.WAVY, ref textOutputString, ref index, value);
                        break;
                    case "pulse":
                        ParseTextEffect(EffectType.PULSE, ref textOutputString, ref index, value);
                        break;
                    case "swivel":
                        ParseTextEffect(EffectType.SWIVEL, ref textOutputString, ref index, value);
                        break;
                    case "jitter":
                        ParseTextEffect(EffectType.JITTER, ref textOutputString, ref index, value);
                        break;
                    case "rainbow":
                        ParseTextEffect(EffectType.RAINBOW, ref textOutputString, ref index, value);
                        break;
                    // Speed modifiers
                    case "speed":
                        ParseSpeedModifier(SpeedModifier.SPEED, index, value);
                        break;
                    case "pause":
                        ParseSpeedModifier(SpeedModifier.PAUSE, index, value);
                        break;
                    case "nlpause":
                        ParseSpeedModifier(SpeedModifier.NEWLINEPAUSE, index, value);
                        break;
                }
            }
            else if (textInputString[i] == '\n')
            {
                ParseSpeedModifier(SpeedModifier.NEWLINE, index, "");
                textOutputString += textInputString[i];
                index += 1;
            }
            else
            {
                if (effectType != EffectType.NONE)
                {
                    TextEffect effect;
                    switch (effectType)
                    {
                        case EffectType.JITTER:
                            effect = new Jitter();
                            break;
                        case EffectType.WAVY:
                            effect = new Wavy();
                            break;
                        case EffectType.PULSE:
                            effect = new Pulse();
                            break;
                        case EffectType.SWIVEL:
                            effect = new Swivel();
                            break;
                        case EffectType.RAINBOW:
                            effect = new Rainbow();
                            break;
                        case EffectType.NONE:
                        default:
                            // Should not be here - just default to rainbow?
                            Debug.LogError("Effect type not set up for default!");
                            effect = new Rainbow();
                            break;
                    }
                    effect.index = textOutputString.Length;
                    effect.strength = effectStrength;
                    effects.Add(effect);
                }
                textOutputString += textInputString[i];
                index += 1;
            }
        }
    }

    private void ParseTextEffect(EffectType type, ref string textOutputString, ref int index, string value)
    {
        string[] splitString = value.Split(seperator, System.StringSplitOptions.RemoveEmptyEntries);
        string word = splitString[0];
        foreach (char letter in word)
        {
            TextEffect effect;
            switch (type)
            {
                case EffectType.WAVY:
                    effect = new Wavy();
                    break;
                case EffectType.PULSE:
                    effect = new Pulse();
                    break;
                case EffectType.SWIVEL:
                    effect = new Swivel();
                    break;
                case EffectType.JITTER:
                    effect = new Jitter();
                    break;
                case EffectType.RAINBOW:
                default:
                    effect = new Rainbow();
                    break;
            }

            effect.index = index;
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
            index += 1;
        }
    }

    private void ParseTextCreator(CreateType type, int index, string value)
    {
        Creator temp = new Creator();
        temp.index = index;
        Debug.Log("Creator:" + type + ", " + index);
        temp.CreateType = type;
        if (!string.IsNullOrEmpty(value) && type != CreateType.INSTANT)
        {
            temp.time = float.Parse(value);
        }
        else
        {
            temp.time = -1;
        }
        numCreators += 1;
        creatorIndexes.Add(temp);
    }

    private void ParseSpeedModifier(SpeedModifier type, int index, string value)
    {
        SpeedOption temp = new SpeedOption();
        temp.type = type;
        temp.index = index;
        if (!string.IsNullOrEmpty(value))
        {
            temp.value = float.Parse(value);
        }
        speeds.Add(temp);
        numSpeeds += 1;
    }


    IEnumerator DisplayText()
    {
        charIndex = 0;
        displayedCharIndex = 0;
        int speedIndex = 0;
        int creatorsIndex = 0;
        while (charIndex < textOutputString.Length)
        {
            if (speedIndex < numSpeeds && speeds[speedIndex].index == displayedCharIndex)
            {
                if (speeds[speedIndex].type == SpeedModifier.SPEED)
                {
                    charDelay = speeds[speedIndex].value;
                }
                if (speeds[speedIndex].type == SpeedModifier.NEWLINEPAUSE)
                {
                    newLinePause = speeds[speedIndex].value;
                }
                if (speeds[speedIndex].type == SpeedModifier.NEWLINE)
                {
                    yield return new WaitForSeconds(newLinePause);
                }
                if (speeds[speedIndex].type == SpeedModifier.PAUSE)
                {
                    yield return new WaitForSeconds(speeds[speedIndex].value);
                }
                speedIndex += 1;
            }
            else if (creatorsIndex < numCreators && creatorIndexes[creatorsIndex].index == displayedCharIndex)
            {
                if (creatorIndexes[creatorsIndex].CreateType == CreateType.INSTANT)
                {
                    createtype = CreateType.INSTANT;
                }
                if (creatorIndexes[creatorsIndex].CreateType == CreateType.FADEIN)
                {
                    createtype = CreateType.FADEIN;
                    if (creatorIndexes[creatorsIndex].time != -1f)
                    {
                        createTime = creatorIndexes[creatorsIndex].time;
                    }
                }
                if (creatorIndexes[creatorsIndex].CreateType == CreateType.POP)
                {
                    createtype = CreateType.POP;
                    if (creatorIndexes[creatorsIndex].time != -1f)
                    {
                        createTime = creatorIndexes[creatorsIndex].time;
                    }
                }
                if (creatorIndexes[creatorsIndex].CreateType == CreateType.FLIP)
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
                    displayedCharIndex += 1;
                }
                else if (letter == '<')
                {
                    while (letter != '>')
                    {
                        charIndex += 1;
                        letter = textOutputString[charIndex];
                    }
                    charIndex += 1;
                    letter = textOutputString[charIndex];
                }

                else
                {
                    Debug.Log(letter + ", " + charIndex + ", " + displayedCharIndex);
                    TextCreator temp;
                    switch (createtype)
                    {
                        case CreateType.INSTANT:
                            charIndex += 1;
                            displayedCharIndex += 1;
                            break;
                        case CreateType.FADEIN:
                            temp = new FadeIn();
                            temp.index = displayedCharIndex;
                            temp.startTime = Time.time;
                            temp.duration = createTime;
                            temp.endColor = startColor;
                            creators.Add(temp);
                            charIndex += 1;
                            displayedCharIndex += 1;
                            break;
                        case CreateType.POP:
                            temp = new Pop();
                            temp.index = displayedCharIndex;
                            temp.startTime = Time.time;
                            temp.duration = createTime;
                            temp.endColor = startColor;
                            creators.Add(temp);
                            charIndex += 1;
                            displayedCharIndex += 1;
                            break;
                        case CreateType.FLIP:
                            temp = new Flip();
                            temp.index = displayedCharIndex;
                            temp.startTime = Time.time;
                            temp.duration = createTime;
                            temp.endColor = startColor;
                            creators.Add(temp);
                            charIndex += 1;
                            displayedCharIndex += 1;
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
        if (revealing)
        {
            StopCoroutine("DisplayText");
        }
        else { revealing = true; }

        // Resetting all variables
        if (m_TextComponent == null)
        {
            m_TextComponent = GetComponent<TMP_Text>();
        }
        startColor = m_TextComponent.color;
        charIndex = 0;
        speeds.Clear();
        numSpeeds = 0;
        effects.Clear();
        creators.Clear();
        creatorIndexes.Clear();
        numCreators = 0;

        textInputString = text;
        ParseText();
        m_TextComponent.text = textOutputString;
        startColor = m_TextComponent.color;
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
            displayedCharIndex = m_TextComponent.textInfo.characterCount;
            creators.Clear();
            creatorIndexes.Clear();
            speeds.Clear();
            revealing = false;
        }
    }
}


