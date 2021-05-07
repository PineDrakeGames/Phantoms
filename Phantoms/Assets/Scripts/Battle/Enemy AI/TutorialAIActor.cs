using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Ares {
    [AddComponentMenu("Ares/Tutorial AI Actor", 2)]
    public class TutorialAIActor : AIActor
    {
        public string Conversation = null;
		BattleDelayElement battleDelayer = null;

		private void Start()
		{
			battleDelayer = gameObject.AddComponent<BattleDelayElement>();
			battleDelayer.LinkToBattle(Battle);
			battleDelayer.RequestBattleDelayLock(DelayRequestReason.UIEvent);

			PixelCrushers.DialogueSystem.DialogueManager.StartConversation(Conversation);

			PixelCrushers.DialogueSystem.DialogueManager.instance.conversationEnded += OnConversationEnd;
		}

		public void OnConversationEnd(Transform transform = null)
		{
			battleDelayer.ReleaseBattleDelayLock();
			PixelCrushers.DialogueSystem.DialogueManager.instance.conversationEnded -= OnConversationEnd;
		}

        public override void SelectAction(ActionInput actionInput){
			VerboseLogger.Log("Selecting AI action");

            // TODO: Set up targeting to make sure that no matter what, the player doesn't die.
            

			if(actionInput.ValidAbilities.Length > 0){
				actionInput.AbilitySelectCallback(actionInput.ValidAbilities[Random.Range(0, actionInput.ValidAbilities.Length)]);
			}
			else if(actionInput.ValidAbilities.Length > 0){
				actionInput.ItemSelectCallback(actionInput.ValidItems[Random.Range(0, actionInput.ValidItems.Length)]);
			}
			else{
				actionInput.SkipCallback();
			}
		}

		public override void SelectTarget(Ability ability, TargetInputSingleActor targetInput){
			VerboseLogger.Log(string.Format("Selecting AI target for ability {0}", ability.Data.DisplayName));

            // TODO: Set up targeting to make sure that no matter what, the player doesn't die.

			targetInput.TargetSelectCallback(targetInput.ValidTargets[Random.Range(0, targetInput.ValidTargets.Length)]);
		}

		public override void SelectTargets(Ability ability, TargetInputNumActors targetInput){
			// Probably will never be used? I imagine the phantom will mostly just single target things
			List<int> availableTargetIndices = Enumerable.Range(0, targetInput.ValidTargets.Length).ToList();
			Actor[] chosenTargets = new Actor[Mathf.Min(targetInput.TargetsRequired, targetInput.ValidTargets.Length)];

			for(int i = 0; i < chosenTargets.Length; i++){
				int chosenIndex = Random.Range(0, availableTargetIndices.Count);

				chosenTargets[i] = targetInput.ValidTargets[availableTargetIndices[chosenIndex]];
				availableTargetIndices.RemoveAt(chosenIndex);
			}

			targetInput.TargetSelectCallback(chosenTargets);
		}

		public override void SelectTargets(Ability ability, TargetInputGroup targetInput){
			// Also assume that this will never be used probably...
			targetInput.TargetSelectCallback(targetInput.ValidTargets[Random.Range(0, targetInput.ValidTargets.Length)]);
		}

		public override void SelectTarget(Item item, TargetInputSingleActor targetInput){
			// Dont think the tutorial phantom would ever use items, but just in case...
			targetInput.TargetSelectCallback(targetInput.ValidTargets[Random.Range(0, targetInput.ValidTargets.Length)]);
		}

		public override void SelectTargets(Item item, TargetInputNumActors targetInput){
			// Dont think the tutorial phantom would ever use items, but just in case...
			List<int> availableTargetIndices = Enumerable.Range(0, targetInput.ValidTargets.Length).ToList();
			Actor[] chosenTargets = new Actor[targetInput.TargetsRequired];

			for(int i = 0; i < targetInput.TargetsRequired; i++){
				int chosenIndex = Random.Range(0, availableTargetIndices.Count);

				chosenTargets[i] = targetInput.ValidTargets[availableTargetIndices[chosenIndex]];
				availableTargetIndices.ToList().RemoveAt(chosenIndex);
			}

			targetInput.TargetSelectCallback(chosenTargets);
		}

		public override void SelectTargets(Item item, TargetInputGroup targetInput){
			// Dont think the tutorial phantom would ever use items, but just in case...
			targetInput.TargetSelectCallback(targetInput.ValidTargets[Random.Range(0, targetInput.ValidTargets.Length)]);
		}
    }
}