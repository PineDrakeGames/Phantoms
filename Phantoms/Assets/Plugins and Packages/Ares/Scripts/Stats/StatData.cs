using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Ares {
	public enum StatStackingType
	{
		Scaling,
		PercentMultipliers
	}

	[CreateAssetMenu(fileName="New Ares Stat", menuName="Ares/Stat", order=30)]
	public class StatData : PowerData {
		public static StatData[] All{
			get{
				#if UNITY_EDITOR
				return Resources.LoadAll<StatData>("Stats"); //always reload
				#else
				if(all == null){
					all = Resources.LoadAll<StatData>("Stats");
				}

				return all;
				#endif
			}
		}

		static StatData[] all;

		public string DisplayName {get{return displayName;}}
		public StatStackingType StackingType {get{return stackingType;}}

		public float GetValue(int baseValue, int stage){
			return GetScaledPowerFloat(baseValue, stage, true);
		}

		[SerializeField] string displayName = null;
		[SerializeField] StatStackingType stackingType = StatStackingType.Scaling;
	}

	[System.Serializable]
	public class Stat {

		public class StatBuffData
		{
			public int Amount = 0;
			public string BuffID = "";

			public StatBuffData(string buffID, int amount)
			{
				BuffID = buffID;
				Amount = amount;
			}
		}

		public StatData Data {get{return data;}}
		public float Value {
			get
			{
				switch(data.StackingType)
				{
					case StatStackingType.Scaling:
						return data.GetValue(baseValue, stage);
					case StatStackingType.PercentMultipliers:
						return GetMultipliedValue();
				}
				return 0;
			}
		}

		// Scaling stacking values
		public int Stage {get{return Mathf.Clamp(stage, data.MinStage, data.MaxStage);}}
		public int baseValue = 50;

		[SerializeField] StatData data = null;
		[SerializeField] int stage = 0;

		[SerializeField] List<StatBuffData> statBuffs = new List<StatBuffData>();

		public Stat(StatData data){
			this.data = data;
		}

		public void Buff(string buffID, int stages){
			statBuffs.Add(new StatBuffData(buffID, stages));
			stage = stage + stages;
		}

		public int ClearBuff(string buffID){
			for (int i = 0; i < statBuffs.Count; i++)
			{
				StatBuffData buffData = statBuffs[i];
				if (buffData.BuffID == buffID)
				{
					statBuffs.Remove(buffData);
					stage = stage - buffData.Amount;
					return buffData.Amount;
				}
			}
			return 0;
		}

		public float GetMultipliedValue()
		{
			float hitChance = 100f;

			foreach(StatBuffData buffData in statBuffs)
			{
				hitChance = hitChance - (hitChance * ((float)buffData.Amount / 100f));
			}

			return (100f - hitChance);
		}

		public void Reset()
		{
			stage = 0;
		}
	}
}