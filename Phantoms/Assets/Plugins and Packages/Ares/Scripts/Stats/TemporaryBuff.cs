using UnityEngine;

namespace Ares {
	public class TemporaryBuff {
		public StatData Stat {get{return stat;}}

		public int Stages {get{return stages;}}
		public string BuffID { get{return buffID;}}
		public int TurnsRemaining 
		{
			get{return turnsRemaining;} 
			set
			{
				turnsRemaining = Mathf.Max(0, value);
				TurnsRemainingUpdate.Invoke(turnsRemaining);
			}
		}

		public IntEvent TurnsRemainingUpdate = new IntEvent();

		[SerializeField] StatData stat;
		[SerializeField] int stages = 0;
		[SerializeField] string buffID = "";
		[SerializeField] int turnsRemaining = 0;

		public TemporaryBuff(StatData stat, string buffID, int stages, int duration){
			this.stat = stat;
			this.buffID = buffID;
			this.stages = stages;
			turnsRemaining = duration;
		}
	}
}