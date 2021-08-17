using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private static Player s_instance = null;
    public static Player Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = FindObjectOfType<Player>();
            }
            return s_instance;
        }
    }

    public PlayerController Character;
    public CameraController CharacterCamera;

    private const string MouseXInput = "Mouse X";
    private const string MouseYInput = "Mouse Y";
    private const string MouseScrollInput = "Mouse ScrollWheel";
    private const string HorizontalInput = "Horizontal";
    private const string VerticalInput = "Vertical";

    private void Awake()
    {
        if (s_instance == null) { s_instance = this; }
        if (s_instance != this)
        {
            return;
        }

        Initialize();
    }

    public void Initialize()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        if (Character == null)
        {
            Character = FindObjectOfType<PlayerController>();
        }
        if (CharacterCamera == null)
        {
            CharacterCamera = FindObjectOfType<CameraController>();
        }
        if (CharacterCamera != null)
        {
            CharacterCamera.Player = Character.CameraFollowPoint;
            CharacterCamera.PlayerMotor = Character.Motor;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //Cursor.lockState = CursorLockMode.Locked;
        }

        HandleCharacterInput();
    }

    private void HandleCharacterInput()
    {
        PlayerCharacterInputs characterInputs = new PlayerCharacterInputs();

        // Build the CharacterInputs struct
        characterInputs.MoveAxisForward = Input.GetAxisRaw(VerticalInput);
        characterInputs.MoveAxisRight = Input.GetAxisRaw(HorizontalInput);
        characterInputs.CameraRotation = CharacterCamera.transform.rotation;
        characterInputs.JumpDown = Input.GetKeyDown(KeyCode.Space);
        characterInputs.JumpHeld = Input.GetKey(KeyCode.Space);
        characterInputs.InteractDown = Input.GetKeyDown(KeyCode.E);
        characterInputs.AttackDown = Input.GetMouseButtonDown(0);

        // Apply inputs to character
        Character.SetInputs(ref characterInputs);
    }
}
