using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Phantoms/Enemy Encounter Data/Tutorial Encounter")]
public class TutorialEnemyEncounter : EnemyEncounterData
{
    [SerializeField]
    private PhantomInstanceData m_phantom = new PhantomInstanceData();

    [SerializeField] [PixelCrushers.DialogueSystem.ConversationPopup(true)] private string m_conversationToPlay = null;

    public override List<Ares.Actor> GetEnemies()
    {
        List<Ares.Actor> phantomActors = new List<Ares.Actor>();

        m_phantom.SetCurrentStats();
        m_phantom.FullRestore();

        GameObject spawnedPhantom = Instantiate(m_phantom.Data.BattlePrefab, Vector3.zero, Quaternion.identity);

        Ares.TutorialAIActor actorComponent = spawnedPhantom.GetComponent<Ares.TutorialAIActor>();
        if (actorComponent == null)
        {
            actorComponent = spawnedPhantom.AddComponent<Ares.TutorialAIActor>();
        }
        actorComponent.Conversation = m_conversationToPlay;

        SetPhantomActor(spawnedPhantom, actorComponent, m_phantom);

        phantomActors.Add(actorComponent);

        return phantomActors;
    }
}
