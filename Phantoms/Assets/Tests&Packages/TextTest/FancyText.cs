using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FancyText : MonoBehaviour
{


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
        for (int i = charIndex; i < characterCount; i += 1)
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
        for (int i = 0; i < textInputString.Length; i++)
        {
            if (textInputString[i] == '<')
            {
                i += 1;
                while (textInputString[i] != '>')
                {
                    i += 1;
                }
                i += 1;
            }
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
                    }
                    else
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
                        }
                        else
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
                temp.index = textOutputString.Length; ;
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
            }
            else if (creatorsIndex < numCreators && creatorIndexes[creatorsIndex].index == charIndex)
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
            creators.Clear();
            creatorIndexes.Clear();
            speeds.Clear();
            revealing = false;
        }
    }
}


