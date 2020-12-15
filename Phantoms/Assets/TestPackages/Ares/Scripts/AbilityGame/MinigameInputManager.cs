using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Enum of all possible inputs
// Can either be on press, on release, or on hold.
public enum MinigameInput
{
    LEFT,
    RIGHT,
    UP,
    DOWN,
    JUMP,
    BUTTON1,
    BUTTON2
}

public static class MinigameInputManager
{
    public static Dictionary<MinigameInput, List<KeyCode>> InputToKeyCodes = new Dictionary<MinigameInput, List<KeyCode>>()
    {
        { MinigameInput.LEFT, new List<KeyCode>(){KeyCode.A, KeyCode.LeftArrow} },
        { MinigameInput.RIGHT, new List<KeyCode>(){KeyCode.D, KeyCode.RightArrow} },
        { MinigameInput.UP, new List<KeyCode>(){KeyCode.W, KeyCode.UpArrow} },
        { MinigameInput.DOWN, new List<KeyCode>(){KeyCode.S, KeyCode.DownArrow} },
        { MinigameInput.JUMP, new List<KeyCode>(){KeyCode.Space} },
        { MinigameInput.BUTTON1, new List<KeyCode>(){KeyCode.Q, KeyCode.Mouse0} },
        { MinigameInput.BUTTON2, new List<KeyCode>(){KeyCode.E, KeyCode.Mouse1} }
    };

    public static bool CheckInputDown(MinigameInput input)
    {
        // Check Keyboard Input
        foreach(KeyCode key in InputToKeyCodes[input])
        {
            if (Input.GetKeyDown(key))
            {
                return true;
            }
        }
        return false;
    }

    public static bool CheckInputHold(MinigameInput input)
    {
        // Check Keyboard Input
        foreach(KeyCode key in InputToKeyCodes[input])
        {
            if (Input.GetKey(key))
            {
                return true;
            }
        }
        return false;
    }

    public static bool CheckInputUp(MinigameInput input)
    {
        // Check Keyboard Input
        foreach(KeyCode key in InputToKeyCodes[input])
        {
            if (Input.GetKeyUp(key))
            {
                return true;
            }
        }
        return false;
    }
}
