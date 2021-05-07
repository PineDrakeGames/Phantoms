using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Phantoms/Enemy Encounter Data/Catch Tutorial Encounter")]
public class CatchTutorialEnemyEncounter : EnemyEncounterData
{
    [SerializeField]
    private PhantomInstanceData m_phantom = new PhantomInstanceData();

    [SerializeField] [PixelCrushers.DialogueSystem.ConversationPopup(true)] private string m_startConversation = null;
    [SerializeField] [PixelCrushers.DialogueSystem.ConversationPopup(true)] private string m_catchConversation = null;

    public override Dictionary<CombatantInstanceData, Ares.Actor> GetEnemies()
    {
        Dictionary<CombatantInstanceData, Ares.Actor> phantomActors = new Dictionary<CombatantInstanceData, Ares.Actor>();

        m_phantom.SetCurrentStats();
        m_phantom.FullRestore();

        GameObject spawnedPhantom = Instantiate(m_phantom.Data.BattlePrefab, Vector3.zero, Quaternion.identity);

        Ares.CatchTutorialAIActor actorComponent = spawnedPhantom.GetComponent<Ares.CatchTutorialAIActor>();
        if (actorComponent == null)
        {
            actorComponent = spawnedPhantom.AddComponent<Ares.CatchTutorialAIActor>();
        }
        actorComponent.StartConversation = m_startConversation;
        actorComponent.CatchConversation = m_catchConversation;

        SetPhantomActor(spawnedPhantom, actorComponent, m_phantom);

        phantomActors[m_phantom] = actorComponent;

        return phantomActors;
    }
}
