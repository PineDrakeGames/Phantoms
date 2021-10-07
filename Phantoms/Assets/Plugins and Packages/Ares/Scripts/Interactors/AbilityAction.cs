using UnityEngine;

namespace Ares {
	[System.Serializable]
	public class AbilityAction : ChainableAction {
		public enum TargetType {ChosenTarget, Self, RandomEnemy, RandomAlly}

		public TargetType TargetMode {get{return targetType;}}
		public bool MissOnMinigameFail { get { return missOnMinigameFail;}}

		[SerializeField] TargetType targetType = TargetType.ChosenTarget;
		[SerializeField] bool missOnMinigameFail = false;
	}
}