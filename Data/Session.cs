using HamstarHelpers.Helpers.DebugHelpers;
using Newtonsoft.Json;
using System.Collections.Generic;


namespace ResetMode.Data {
	public class ResetModeSessionData {
		public ISet<string> PlayersValidated = new HashSet<string>();
		public IDictionary<string, float> PlayerPPSpendings = new Dictionary<string, float>();

		public ISet<string> AllPlayedWorlds = new HashSet<string>();
		public string CurrentSessionedWorldId = "";

		public bool AwaitingNextWorld = false;
		public bool IsRunning = false;



		////////////////
		
		internal void ResetAll() {
			this.PlayersValidated.Clear();
			this.AllPlayedWorlds.Clear();
			this.PlayerPPSpendings.Clear();
			this.CurrentSessionedWorldId = "";
			this.AwaitingNextWorld = false;
			this.IsRunning = false;

			if( ResetModeMod.Instance.Config.DebugModeInfo ) {
				LogHelpers.Log( "ResetMode.Data.ResetModeSessionData" );
			}
		}


		internal ResetModeSessionData Clone() {
			return (ResetModeSessionData)this.MemberwiseClone();
		}

		////////////////

		public override string ToString() {
			return JsonConvert.SerializeObject( this );
		}
	}
}
