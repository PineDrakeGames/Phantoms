using UnityEngine;

// Added by CJ!

namespace PixelCrushers.DialogueSystem.SequencerCommands
{

    /// <summary>
    /// Implements sequencer command: TogglePlayerControl(enabled)
    /// 
    /// Arguments:
    /// -# whether or not the player has control!
    /// </summary>
    [AddComponentMenu("")] // Hide from menu.
    public class SequencerCommandTogglePlayerControl : SequencerCommand
    {
        public void Start()
        {
            PlayerController controller = null;
            
            if (OverworldManager.Instance != null)
            {
                controller = OverworldManager.Instance.PlayerController;
                if (controller == null)
                {
                    Stop();
                }
            }
            else
            {
                Stop();
            }

            // Get the values of the parameters:
            string enabled = GetParameter(0, "null");
            enabled = enabled.ToLower().Trim();
            bool playerShouldHaveControl = (controller.CurrentState is PlayerStateInteract);
            if (enabled == "true") { playerShouldHaveControl = true; }
            if (enabled == "false") { playerShouldHaveControl = false; }

            if (playerShouldHaveControl)
            {
                controller.SetState(new PlayerStateIdle());
            }
            else
            {
                controller.SetState(new PlayerStateInteract());
            }

            Stop();
        }
    }
}