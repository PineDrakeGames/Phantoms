using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerController Character;
    public CameraController CharacterCamera;

    private const string MouseXInput = "Mouse X";
    private const string MouseYInput = "Mouse Y";
    private const string MouseScrollInput = "Mouse ScrollWheel";
    private const string HorizontalInput = "Horizontal";
    private const string VerticalInput = "Vertical";

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        CharacterCamera.Player = Character.CameraFollowPoint;
        CharacterCamera.PlayerMotor = Character.Motor;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Cursor.lockState = CursorLockMode.Locked;
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
        characterInputs.CrouchDown = Input.GetKeyDown(KeyCode.C);
        characterInputs.CrouchUp = Input.GetKeyUp(KeyCode.C);

        // Apply inputs to character
        Character.SetInputs(ref characterInputs);
    }
}
