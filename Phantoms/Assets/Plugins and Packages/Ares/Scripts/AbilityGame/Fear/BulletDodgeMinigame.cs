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

    [Header("Scene References")]
    [SerializeField]
    private BulletDodgeMinigamePlayer m_player;
    [SerializeField]
    private RectTransform m_playerArea = null;
    [SerializeField]
    private MinigameHealthIndicator m_healthIndicator = null;
    [SerializeField]
    private Transform m_bulletParent = null;

    [Header("Prefabs to Instantiate")]
    [SerializeField]
    private GameObject m_bulletPrefab;


    /// Private variables
    private float m_currentTime = 0f;

    private List<BulletSpawner> Spawners = new List<BulletSpawner>();

    private List<BulletDodgeMinigameBullet> m_bullets = new List<BulletDodgeMinigameBullet>();

    ////////////////////////////////////
    /// Protected override functions ///
    ////////////////////////////////////
    protected override void Restart()
    {
        Spawners.Clear();
    }

    protected override void InitializingState()
    {

        m_currentTime = 0f;

        m_player.PlayerSpeed = Data.PlayerSpeed;
        m_player.AllowMovement = true;
        m_healthIndicator.SetMaxHearts(Data.Health);

        foreach (BulletSpawnerData data in Data.BulletSpawners)
        {
            BulletSpawner spawner = new BulletSpawner();
            spawner.Data = data;
            spawner.TimeToNextSpawn = data.StartDelay;
            Spawners.Add(spawner);
        }

        m_state = MinigameState.RUNNING;
    }

    protected override void RunningState()
    {
        foreach(BulletSpawner spawner in Spawners)
        {
            UpdateSpawner(spawner);
        }

        Canvas.ForceUpdateCanvases();

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

    private void UpdateSpawner(BulletSpawner spawner)
    {
        spawner.TimeToNextSpawn -= Time.deltaTime;
        while (spawner.TimeToNextSpawn <= 0)
        {
            // Spawn the bullet
            Vector2 spawnPoint = Vector2.zero;
            Vector2 defaultSpawnDirection = Vector2.zero;
            switch(spawner.Data.Spawn)
            {
                case BulletSpawnerData.SpawnPoint.BOTTOM:
                    defaultSpawnDirection = Vector2.up;
                    spawnPoint.y = m_playerArea.rect.yMin;
                    spawnPoint.x = Random.Range(m_playerArea.rect.xMin, m_playerArea.rect.xMax);
                    break;

                case BulletSpawnerData.SpawnPoint.TOP:
                    defaultSpawnDirection = Vector2.down;
                    spawnPoint.y = m_playerArea.rect.yMax;
                    spawnPoint.x = Random.Range(m_playerArea.rect.xMin, m_playerArea.rect.xMax);
                    break;

                case BulletSpawnerData.SpawnPoint.RIGHT:
                    defaultSpawnDirection = Vector2.left;
                    spawnPoint.x = m_playerArea.rect.xMax;
                    spawnPoint.y = Random.Range(m_playerArea.rect.yMin, m_playerArea.rect.yMax);
                    break;

                case BulletSpawnerData.SpawnPoint.LEFT:
                    defaultSpawnDirection = Vector2.right;
                    spawnPoint.x = m_playerArea.rect.xMin;
                    spawnPoint.y = Random.Range(m_playerArea.rect.yMin, m_playerArea.rect.yMax);
                    break;
            }

            BulletDodgeMinigameBullet bullet = GetBullet();
            bullet.SetPosition(spawnPoint);

            float rotation = spawner.Data.BulletStartAngle + (Random.Range(-1f, 1f) * spawner.Data.BulletStartAngleVariance);
            Vector2 direction = Rotate(defaultSpawnDirection, rotation);

            float speed = spawner.Data.BulletStartSpeed + (Random.Range(-1f, 1f) * spawner.Data.BulletStartSpeedVariance);
            float lifetime = spawner.Data.BulletLifetime + (Random.Range(-1f, 1f) * spawner.Data.BulletLifetimeVariance);

            bullet.SetLinearMovemet(direction, speed, lifetime);

            // Set the time to the next bullet
            float spawnDelay = spawner.Data.BulletSpawnInterval + (Random.Range(-1f, 1f) * spawner.Data.BulletSpawnIntervalVariance);

            spawner.TimeToNextSpawn += spawnDelay;
            
        }
    }

    private BulletDodgeMinigameBullet GetBullet()
    {
        // TODO: Pooling!
        BulletDodgeMinigameBullet returnBullet = null;
        foreach(BulletDodgeMinigameBullet bullet in m_bullets)
        {
            if (!bullet.gameObject.activeSelf)
            {
                bullet.gameObject.SetActive(true);
                returnBullet = bullet;
                break;
            }
        }
        if (returnBullet == null)
        {
            GameObject bulletObject = Instantiate(m_bulletPrefab, m_bulletParent);
            returnBullet = bulletObject.GetComponent<BulletDodgeMinigameBullet>();
            returnBullet.BulletMinigame = this;
            m_bullets.Add(returnBullet);
        }
        return returnBullet;
    }

    private Vector2 Rotate(Vector2 v, float delta)
    {
        return new Vector2(
            v.x * Mathf.Cos(delta * Mathf.Deg2Rad) - v.y * Mathf.Sin(delta * Mathf.Deg2Rad),
            v.x * Mathf.Sin(delta * Mathf.Deg2Rad) + v.y * Mathf.Cos(delta * Mathf.Deg2Rad)
        );
    }
}
