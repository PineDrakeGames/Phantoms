using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoveMinigame : AbilityMinigame
{

    [Header("Minigame Variables")]
    public LoveMinigameData Data;

    [Header("Scene References")]
    [SerializeField]
    private RectTransform m_bulletLauncher = null;
    [SerializeField]
    private RectTransform m_targetArea = null;
    [SerializeField]
    private Transform m_bulletsParent = null;
    [SerializeField]
    private Transform m_targetsParent = null;

    [Header("Prefabs")]
    [SerializeField]
    private GameObject m_bulletPrefab = null;

    [SerializeField]
    private GameObject m_targetPrefab = null;

    public override string MinigameDescription
    {
        get
        {
            return "Aim at the targets!";
        }
    }

    private float m_launcherAngle = 0f;
    private float m_bulletCooldown = 0f;

    private List<LoveMinigameBullet> m_bullets = new List<LoveMinigameBullet>();
    private List<LoveMinigameTarget> m_targets = new List<LoveMinigameTarget>();

    ////////////////////////////////////
    /// Protected override functions ///
    ////////////////////////////////////
    protected override void Restart()
    {

    }

    protected override void InitializingState()
    {
        SetLauncherPosition();
        SpawnRandomTargets(Data.NumTargets);
        AbilityMinigameManager.Timer.StartTimer(Data.TimerDuration);
        m_state = MinigameState.RUNNING;
    }

    protected override void RunningState()
    {
        ControlLauncher();
    }

    protected override void FinishedState()
    {

    }

    /////////////////////////////////////////
    /// Private Target-Spawning Functions ///
    /////////////////////////////////////////

    private void SpawnRandomTargets(int NumTargets)
    {
        List<Vector2> pointOptions = PoissonDiscSampling.GeneratePoints(70f, new Vector2(m_targetArea.rect.width, m_targetArea.rect.height));

        Vector2 bottomLeftCorner = m_targetArea.anchoredPosition - new Vector2(m_targetArea.rect.width / 2f, m_targetArea.rect.height / 2f);

        int targetsSpawned = 0;
        while ((pointOptions.Count > 0) && (targetsSpawned < NumTargets))
        {
            int randomIndex = Random.Range(0,pointOptions.Count);
            Vector2 randomPoint = bottomLeftCorner + pointOptions[randomIndex];

            if (Vector2.Distance(randomPoint, m_bulletLauncher.anchoredPosition) >= Data.MinTargetDistance)
            {
                LoveMinigameTarget target = GetTarget();
                target.SetPosition(randomPoint);
                targetsSpawned += 1;
            }

            pointOptions.RemoveAt(randomIndex);
        }
    }


    //////////////////////////////////
    /// Private Controls Functions ///
    //////////////////////////////////

    private void ControlLauncher()
    {
        
        // TODO: Launcher controls
        float verticalMove = Input.GetAxisRaw("Vertical");
        float horizontalMove = Input.GetAxisRaw("Horizontal") * -1f;

        float clockwiseMove = Mathf.Clamp(verticalMove + horizontalMove, -1f, 1f);

        m_launcherAngle += clockwiseMove * Data.AimSensitivity * Time.deltaTime;

        // If the aim angle is >360, just allow full rotation - otherwise just clamp the value.
        if ((Data.AimAngleRange.maxValue - Data.AimAngleRange.minValue) >= 360f)
        {
            while (m_launcherAngle < 0f) { m_launcherAngle += 360f; }
            while (m_launcherAngle > 360f) { m_launcherAngle -= 360f; }
        }
        else
        {
            m_launcherAngle = Mathf.Clamp(m_launcherAngle, Data.AimAngleRange.minValue, Data.AimAngleRange.maxValue);
        }

        // Set the launcher rotation
        m_bulletLauncher.rotation = Quaternion.Euler(0f, 0f, m_launcherAngle);

        // Update bullet cooldown, and check if we want to fire another.
        if (m_bulletCooldown > 0f)
        {
            m_bulletCooldown -= Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.Space) && m_bulletCooldown <= 0f)
        {
            FireBullet();
        }
    }

    private void FireBullet()
    {
        LoveMinigameBullet bullet = GetBullet();
        bullet.SetPosition(m_bulletLauncher.anchoredPosition);

        Vector2 direction = new Vector2(Mathf.Cos(m_launcherAngle * Mathf.Deg2Rad), Mathf.Sin(m_launcherAngle * Mathf.Deg2Rad));
        bullet.SetMovement(direction, Data.BulletType, Data.BulletSpeed, Data.BulletDuration);

        m_bulletCooldown = (1f / Data.BulletFireRate);
    }

    ////////////////////////////////
    /// Private helper functions ///
    ////////////////////////////////
    private void SetLauncherPosition()
    {
        // NOTE: May need to set different parent? for now these are the same.
        float width = m_targetArea.rect.width;
        float height = m_targetArea.rect.height;
        Vector2 offset = new Vector2(Data.ReticlePosition.x * (width / 2f), Data.ReticlePosition.y * (height / 2f));
        
        m_bulletLauncher.anchoredPosition = m_targetArea.anchoredPosition + offset;

        m_launcherAngle = (Data.AimAngleRange.minValue + Data.AimAngleRange.maxValue) / 2f;

        m_bulletLauncher.rotation = Quaternion.Euler(0f, 0f, m_launcherAngle);
    }

    /////////////////////////////////
    /// Private Pooling Functions ///
    /////////////////////////////////

    private LoveMinigameBullet GetBullet()
    {
        foreach(LoveMinigameBullet bullet in m_bullets)
        {
            if (!bullet.gameObject.activeSelf)
            {
                bullet.gameObject.SetActive(true);
                return bullet;
            }
        }

        GameObject instance = Instantiate(m_bulletPrefab, m_bulletsParent);
        LoveMinigameBullet bulletComponent = instance.GetComponent<LoveMinigameBullet>();
        m_bullets.Add(bulletComponent);

        return bulletComponent;
    }

    private void ClearBullets()
    {
        foreach(LoveMinigameBullet bullet in m_bullets)
        {
            bullet.gameObject.SetActive(false);
        }
    }

    private LoveMinigameTarget GetTarget()
    {
        foreach(LoveMinigameTarget target in m_targets)
        {
            if (!target.gameObject.activeSelf)
            {
                target.gameObject.SetActive(true);
                return target;
            }
        }

        GameObject instance = Instantiate(m_targetPrefab, m_targetsParent);
        LoveMinigameTarget targetComponent = instance.GetComponent<LoveMinigameTarget>();
        m_targets.Add(targetComponent);

        return targetComponent;
    }

    private void ClearTargets()
    {
        foreach(LoveMinigameTarget target in m_targets)
        {
            target.gameObject.SetActive(false);
        }
    }
}
