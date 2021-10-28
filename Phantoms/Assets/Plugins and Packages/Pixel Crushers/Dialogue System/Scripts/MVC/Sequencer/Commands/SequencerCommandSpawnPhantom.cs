// Copyright (c) Pixel Crushers. All rights reserved.

using UnityEngine;

namespace PixelCrushers.DialogueSystem.SequencerCommands
{

    /// <summary>
    /// Implements sequencer command: SpawnPhantom(phantomToSpawn[, locationToSpawn[, name to assign to the phantom [, duration to spawn in the phantom[, should play spawn animation]]]])
    /// </summary>
    [AddComponentMenu("")] // Hide from menu.
    public class SequencerCommandSpawnPhantom : SequencerCommand
    {

        private GameObject spawnedPhantom;

        private float currentTime = 0f;
        private float spawnInTime = 0f;
        private bool playSpawnAnimation = false;

        public void Start()
        {
            string phantomToSpawn = GetParameter(0);
            Transform locationToSpawn = GetSubject(1);
            string spawnedName = GetParameter(2);
            spawnInTime = GetParameterAsFloat(3, 0.5f);
            playSpawnAnimation = GetParameterAsBool(4, true);

            // First, try to get the phantom data and spawn it in.
            if (string.IsNullOrEmpty(phantomToSpawn))
            {
                Stop();
                return;
            }
            phantomToSpawn = phantomToSpawn.Trim().ToUpper();
            if (phantomToSpawn == "KINDRED")
            {
                phantomToSpawn = DataManager.Instance.ChosenStarterID;
            }
            PhantomData phantomData = DataManager.Instance.TryGetPhantomData(phantomToSpawn);

            // For testing - spawn any ol phantom if we cant find one
            if (phantomData == null) { phantomData = DataManager.PhantomData.Data[0]; }

            if (phantomData != null && phantomData.BattlePrefab != null)
            {
                spawnedPhantom = Instantiate(phantomData.BattlePrefab);
            }
            else
            {
                Stop();
                return;
            }

            // At this point the phantom is spawned in - set it up.
            if (locationToSpawn != null)
            {
                spawnedPhantom.transform.position = locationToSpawn.position;
                spawnedPhantom.transform.rotation = locationToSpawn.rotation;
            }
            spawnedName = spawnedName.Trim();
            if (!string.IsNullOrEmpty(spawnedName))
            {
                spawnedPhantom.name = spawnedName;
            }
            if (spawnInTime > 0f)
            {
                spawnedPhantom.transform.localScale = Vector3.zero;
                currentTime = 0f;
            }
            else
            {
                if (playSpawnAnimation)
                {
                    foreach (Animator anim in spawnedPhantom.GetComponentsInChildren<Animator>())
                    {
                        anim.SetTrigger("Spawned");
                    }
                }
                Stop();
            }
        }

        public void Update()
        {
            currentTime += Time.deltaTime;

            float progress = currentTime / spawnInTime;
            spawnedPhantom.transform.localScale = Vector3.one * Mathf.Clamp01(progress);

            if (progress >= 1f)
            {
                if (playSpawnAnimation)
                {
                    foreach (Animator anim in spawnedPhantom.GetComponentsInChildren<Animator>())
                    {
                        anim.SetTrigger("Spawned");
                    }
                }
                Stop();
            }
        }

        public void OnDestroy()
        {
            if (spawnInTime > 0f && currentTime < spawnInTime)
            {
                spawnedPhantom.transform.localScale = Vector3.one;
                if (playSpawnAnimation)
                {
                    foreach (Animator anim in spawnedPhantom.GetComponentsInChildren<Animator>())
                    {
                        anim.SetTrigger("Spawned");
                    }
                }
            }
        }
    }

}
