using UnityEngine;

public abstract class PlayerMovementState
{
    public PlayerController Controller;

    public abstract void StateEnter();
    public abstract void TickInput(PlayerCharacterInputs input);
    public abstract void TickVelocity(ref Vector3 currentVelocity, float deltaTime);
    public abstract void StateExit();    
}
