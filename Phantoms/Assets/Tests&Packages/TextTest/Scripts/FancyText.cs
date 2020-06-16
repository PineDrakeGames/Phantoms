using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FancyText : MonoBehaviour
{
    /// Enums ///
    public enum SpeedModifier
    {
        SPEED,
        PAUSE,
        NEWLINEPAUSE,
        NEWLINE
    };


    ////  Serialized items  ////
    [Header("Text Effect Variables")]
    [SerializeField]
    private FancyTextEffectTable m_effectTable = null;


    [Tooltip("The default effect type to use if no specific effect type is assigned for a character")]
    public FancyTextEffect defaultEffect = null;

    [Tooltip("The default effect strength for an animation")]
    public float effectStrength = 1f;


    [Header("Character Creation Variables")]
    [Tooltip("The default create effect for this text.")]
    public FancyTextEffect createtype = null;

    [Tooltip("The delay between when each character is revealed.")]
    public float CharacterDelay = 0.05f;


    [Header("Current Status")]
    [ShowOnly]
    public bool Active = false;

    [ShowOnly]
    public bool Revealing = false;


    /// Private variables ///
    // Reveal text strings and things 
    TMP_Text m_TextComponent;
    string textInputString;
    string textOutputString;
    string textDisplayString;
    int charIndex = 0;
    float progress;

    // Things related to text display speed and pauses
    List<SpeedOption> speeds = new List<SpeedOption>();
    int numSpeeds = 0;
    public float newLinePause = 0.8f;

    // Text Effects stuff
    List<TextEffect> effects = new List<TextEffect>();
    string[] seperator = { ">>" };

    // Text Creation stuff
    List<TextCreator> creators = new List<TextCreator>();
    List<Creator> creatorIndexes = new List<Creator>();
    private float numCreators = 0;

    // Audio Stuff
    AudioSource audioSource;
    bool hasAudio = false;

    /// Initialization ///
    // Get references to any components needed in start
    private void Start()
    {
        m_TextComponent = GetComponent<TMP_Text>();
        m_TextComponent.enableVertexGradient = true;
        if (GetComponent<AudioSource>() != null)
        {
            audioSource = GetComponent<AudioSource>();
            hasAudio = true;
        }
    }

    // Update is called once per frame
    private void Update()
    {
        //tell the mesh that the verts must be redrawn
        if (Active)
        {
            ModifyMesh();
        }
    }

    private void ModifyMesh()
    {
        m_TextComponent.ForceMeshUpdate();
        TMP_TextInfo textInfo = m_TextComponent.textInfo;

        // Declare all the variables needed for mesh modification first
        TMP_MeshInfo[] cachedMeshInfo = textInfo.CopyMeshInfoVertexData();
        int characterCount = textInfo.characterCount;
        Color32[] newVertexColors;
        int materialIndex = 0;
        int vertexIndex = 0;
        Vector3[] sourceVertices = null;
        Vector3[] destinationVertices = null;

        // Apply the effects to letters first, in case they are overridden by later changes
        foreach (TextEffect effect in effects)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[effect.index];

            // Skip characters that are not visible and thus have no geometry to manipulate.
            if (!charInfo.isVisible)
                continue;

            materialIndex = textInfo.characterInfo[effect.index].materialReferenceIndex;
            vertexIndex = textInfo.characterInfo[effect.index].vertexIndex;
            sourceVertices = cachedMeshInfo[materialIndex].vertices;
            destinationVertices = textInfo.meshInfo[materialIndex].vertices;
            newVertexColors = textInfo.meshInfo[materialIndex].colors32;

            effect.Apply(vertexIndex, sourceVertices, ref destinationVertices, ref newVertexColors);
        }

        // Next we do the animations for characters that are currently being put in.
        // Iterate through it backwards, so we can remove finished creators from the list as we go.
        for (int i = creators.Count - 1; i >= 0; i--)
        {
            TextCreator creator = creators[i];

            TMP_CharacterInfo charInfo = textInfo.characterInfo[creator.index];

            // Skip characters that are not visible and thus have no geometry to manipulate.
            if (!charInfo.isVisible)
                continue;

            materialIndex = textInfo.characterInfo[creator.index].materialReferenceIndex;
            vertexIndex = textInfo.characterInfo[creator.index].vertexIndex;
            sourceVertices = cachedMeshInfo[materialIndex].vertices;
            destinationVertices = textInfo.meshInfo[materialIndex].vertices;
            newVertexColors = textInfo.meshInfo[materialIndex].colors32;

            creator.Apply(Time.time, vertexIndex, sourceVertices, ref destinationVertices, ref newVertexColors);

            // destroy any creators that are finished
            if (creator.Progress(Time.time) >= 1f)
            {
                //Debug.LogFormat("Vertices for character {0} located at:\n1: {1}\n2: {2}\n1: {3}\n4: {4}", creator.index, sourceVertices[vertexIndex + 0], sourceVertices[vertexIndex + 1], sourceVertices[vertexIndex + 2], sourceVertices[vertexIndex + 3]);
                creators.Remove(creator);
                break;
            }
        }

        //finally, set everything that hasn't appeared yet to an invisible color
        for (int i = charIndex; i < characterCount; i += 1)
        {
            materialIndex = textInfo.characterInfo[i].materialReferenceIndex;
            newVertexColors = textInfo.meshInfo[materialIndex].colors32;
            vertexIndex = textInfo.characterInfo[i].vertexIndex;

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
        textDisplayString = "";
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
            }
            // If we detect a bracket, that means what follows should be a text effect option.
            else if (textInputString[i] == '[')
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
                        textDisplayString += '[';
                        index += 1;
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
                    default:
                        FancyTextEffect textEffect = m_effectTable.GetTextEffect(option);
                        ParseTextEffect(textEffect, ref textOutputString, ref textDisplayString, ref index, value);
                        break;
                }
            }
            else if (textInputString[i] == '\n')
            {
                ParseSpeedModifier(SpeedModifier.NEWLINE, index, "");
                textOutputString += textInputString[i];
                textDisplayString += textInputString[i];
                index += 1;
            }
            else
            {
                if (defaultEffect != null)
                {
                    TextEffect effect = new TextEffect();
                    effect.Effect = defaultEffect;

                    effect.index = index;
                    effects.Add(effect);
                }
                textOutputString += textInputString[i];
                textDisplayString += textInputString[i];
                index += 1;
            }
        }
    }

    private void ParseTextEffect(FancyTextEffect textEffect, ref string textOutputString, ref string textDisplayString, ref int index, string value)
    {
        if (textEffect == null)
        {
            return;
        }
        switch (textEffect.EffectType)
        {
            case TextEffectType.CONSTANT:
                foreach (char letter in value)
                {
                    TextEffect effect = new TextEffect();
                    effect.Effect = textEffect;

                    effect.index = index;
                    effects.Add(effect);
                    textOutputString += letter;
                    textDisplayString += letter;
                    index += 1;
                }
                break;
            case TextEffectType.CREATOR:
                Creator temp = new Creator();
                temp.index = index;
                temp.CreateType = textEffect;
                numCreators += 1;
                creatorIndexes.Add(temp);
                break;
        }
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
        int speedIndex = 0;
        int creatorsIndex = 0;

        bool playedSound = false;

        while (charIndex < textDisplayString.Length)
        {
            // Check if there is any speed changes at this index, and set up all of them if so.
            while (speedIndex < numSpeeds && speeds[speedIndex].index == charIndex)
            {
                switch (speeds[speedIndex].type)
                {
                    case SpeedModifier.SPEED:
                        CharacterDelay = speeds[speedIndex].value;
                        break;
                    case SpeedModifier.NEWLINEPAUSE:
                        newLinePause = speeds[speedIndex].value;
                        break;
                    case SpeedModifier.NEWLINE:
                        yield return new WaitForSeconds(newLinePause);
                        break;
                    case SpeedModifier.PAUSE:
                        yield return new WaitForSeconds(speeds[speedIndex].value);
                        break;
                }
                speedIndex += 1;
            }

            // Check if there is a change in the creators at this index, and set it up if so.
            // NOTE(CJ): Should in theory only ever be 1 creator per index, as only the last one would ever be used - 
            //   but using a while loop just in case.
            while (creatorsIndex < numCreators && creatorIndexes[creatorsIndex].index == charIndex)
            {
                Creator creator = creatorIndexes[creatorsIndex];
                createtype = creator.CreateType;
                creatorsIndex += 1;
            }

            // Get the current letter
            char letter = textDisplayString[charIndex];

            // If the current letter is not visible, skip it.
            if (letter == ' ' || letter == '\n')
            {
                charIndex += 1;
            }
            else
            {
                TextCreator temp = new TextCreator();
                temp.Effect = createtype;
                temp.index = charIndex;
                temp.startTime = Time.time;
                creators.Add(temp);
                charIndex += 1;

                if (!playedSound)
                {
                    if (hasAudio) { audioSource.Play(); }
                    playedSound = true;
                }
                if (CharacterDelay != 0f)
                {
                    playedSound = false;
                    yield return new WaitForSeconds(CharacterDelay);
                }
            }
        }
        Revealing = false;
    }

    public void SetText(string text)
    {
        if (!Active) { Active = true; }
        if (Revealing)
        {
            StopCoroutine("DisplayText");
        }
        else { Revealing = true; }

        // Resetting all variables
        if (m_TextComponent == null)
        {
            m_TextComponent = GetComponent<TMP_Text>();
        }
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
        StartCoroutine("DisplayText");
    }

    public void FinishLine()
    {
        if (Revealing)
        {
            StopCoroutine("DisplayText");
            numSpeeds = 0;
            numCreators = 0;
            charIndex = textDisplayString.Length;
            creators.Clear();
            creatorIndexes.Clear();
            speeds.Clear();
            Revealing = false;
        }
    }
}


