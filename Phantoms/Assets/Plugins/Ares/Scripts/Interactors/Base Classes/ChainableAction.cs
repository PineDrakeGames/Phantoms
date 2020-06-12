using UnityEngine;
using System.Text.RegularExpressions;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Ares {
	[Serializable]
	public class ChainableAction {
		public enum PowerType {Constant, Random, Formula}
		public enum EnvironmentVariableSetType {Set, Unset}

		public ChainEvaluator.ActionType Action {get{return action;}}
		public PowerType PowerMode {get{return powerType;}}
		public AfflictionData Affliction {get{return affliction;}}
		public StatData Stat {get{return stat;}}
		public EnvironmentVariableData EnvironmentVariable {get{return environmentVariable;}}
		public EnvironmentVariableSetType EnvironmentVariableSetMode {get{return environmentVariableSetType;}}
		public float HitChance {get{return chance;}}
		public bool IsChildEffect {get{return isChildEffect;}}
		public float Duration {get{return duration;}}
		public float NormalizedProcessTime {get{return normalizedProcessTime;}}
		public bool BreaksChainOnMiss {get{return breaksChainOnMiss;}}

		[SerializeField] ChainEvaluator.ActionType action = ChainEvaluator.ActionType.Damage;
		[SerializeField] PowerType powerType = PowerType.Constant;
		[SerializeField] StatData stat = null;
		[SerializeField] EnvironmentVariableData environmentVariable = null;
		[SerializeField] EnvironmentVariableSetType environmentVariableSetType = EnvironmentVariableSetType.Set;
		[SerializeField] AfflictionData affliction = null;
		[SerializeField] bool isChildEffect = false;
		[SerializeField] float duration = 1f;
		[SerializeField, Range(0f, 1f)] float normalizedProcessTime = 1f;
		[SerializeField, Range(0f, 1f)] float chance = 1f;
		[SerializeField] float power1 = 0f;
		[SerializeField] float power2 = 0f;
		[SerializeField] string powerFormula = null;
		[SerializeField] bool breaksChainOnMiss = false;

		static Regex reFormula = new Regex(@"([A-Z_]+[0-9]*(?![\(A-Z0-9]))"); //Matches variable references like ATTACK1, (but not ABS(...))

		public float EvaluatePower(Dictionary<string, float> evaluatedValues){
			switch(powerType){
				case PowerType.Constant:
					return power1;
				case PowerType.Random:
					return UnityEngine.Random.Range(power1, power2);
				case PowerType.Formula:
					string formula = reFormula.Replace(powerFormula, m => evaluatedValues[m.Value].ToString());

					return FormulaParser.Parse(formula);
			}

			return 0f;
		}
	}
}