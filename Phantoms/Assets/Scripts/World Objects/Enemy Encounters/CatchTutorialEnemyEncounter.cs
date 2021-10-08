using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Phantoms/Enemy Encounter Data/Catch Tutorial Encounter")]
public class CatchTutorialEnemyEncounter : EnemyEncounterData
{
    [System.Serializable]
    private class TutorialPhantom
    {
        public string StarterID = "";
        public PhantomInstanceData PhantomOption1;
        public PhantomInstanceData PhantomOption2;
        public PhantomInstanceData PhantomOption3;
    }

    [SerializeField]
    private PhantomInstanceData m_phantom = new PhantomInstanceData();
    
    [SerializeField]
    private TutorialPhantom[] m_tuorialPhantoms;

    [SerializeField] [PixelCrushers.DialogueSystem.ConversationPopup(true)] private string m_startConversation = null;
    [SerializeField] [PixelCrushers.DialogueSystem.ConversationPopup(true)] private string m_catchConversation = null;

    public override Dictionary<CombatantInstanceData, Ares.Actor> GetEnemies()
    {
        Dictionary<CombatantInstanceData, Ares.Actor> phantomActors = new Dictionary<CombatantInstanceData, Ares.Actor>();

        if (DataManager.Instance != null && !string.IsNullOrEmpty(DataManager.Instance.ChosenStarterID))
        {
            TutorialPhantom tutorialPhantom = null;
            foreach(TutorialPhantom tutphantom in m_tuorialPhantoms)
            {
                if (tutphantom.StarterID == DataManager.Instance.ChosenStarterID)
                {
                    tutorialPhantom = tutphantom;
                    break;
                }
            }

            if (tutorialPhantom != null && DataManager.Instance.StarterChoices != null)
            {
                if (!DataManager.Instance.StarterChoices.Contains(tutorialPhantom.PhantomOption1.Data.ID))
                {
                    m_phantom = tutorialPhantom.PhantomOption1;
                }
                else if (!DataManager.Instance.StarterChoices.Contains(tutorialPhantom.PhantomOption2.Data.ID))
                {
                    m_phantom = tutorialPhantom.PhantomOption2;
                }
                else
                {
                    m_phantom = tutorialPhantom.PhantomOption3;
                }
            }
        }
        
        if (m_phantom == null)
        {
            m_phantom = m_tuorialPhantoms[0].PhantomOption1;
        }

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
