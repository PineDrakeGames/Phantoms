using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BulletDodgeMinigame : AbilityMinigame
{
    public class ButtonMashInstance
    {
        public MinigameInput Input;
        public InputIndicator Indicator = null;

        public ButtonMashInstance(MinigameInput input)
        {
            Input = input;
        }
    }

    [Header("Minigame Variables")]
    public BulletDodgeMinigameData Data;

    [Header("References to the Player")]
    [SerializeField]
    private BulletDodgeMinigamePlayer m_player;

    [Header("Scene References")]
    [SerializeField]
    private MinigameHealthIndicator m_healthIndicator = null;

    [Header("Prefabs to Instantiate")]
    [SerializeField]
    private GameObject m_bulletPrefab;


    /// Private variables
    private float m_currentTime = 0f;

    ////////////////////////////////////
    /// Protected override functions ///
    ////////////////////////////////////
    protected override void Restart()
    {

    }

    protected override void InitializingState()
    {

        m_currentTime = 0f;

        m_player.PlayerSpeed = Data.PlayerSpeed;
        m_player.AllowMovement = true;
        m_healthIndicator.SetMaxHearts(Data.Health);

        m_state = MinigameState.RUNNING;
    }

    protected override void RunningState()
    {

        m_currentTime += Time.deltaTime;
        if (m_currentTime >= Data.TimerDuration)
        {
            //m_state = MinigameState.FINISHED;
        }
    }

    protected override void FinishedState()
    {

    }


    ////////////////////////
    /// Public Functions ///
    ////////////////////////
    public void HitPlayer(BulletDodgeMinigameBullet bullet = null)
    {
        Debug.Log("Hit");
        m_healthIndicator.Damage();
    }


    /////////////////////////
    /// Private Functions ///
    /////////////////////////
    
}
