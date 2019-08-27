using HamstarHelpers.Helpers.Debug;
using Newtonsoft.Json;
using System.Collections.Generic;


namespace ResetMode.Data {
	public class ResetModeSessionData {
		private readonly object MyLock = new object();
		


		////////////////

		public HashSet<string> PlayersValidated = new HashSet<string>();
		public Dictionary<string, float> PlayerPPSpendings = new Dictionary<string, float>();

		public HashSet<string> AllPlayedWorlds = new HashSet<string>();
		public string CurrentSessionedWorldId = "";

		public bool AwaitingNextWorld = false;
		public bool IsRunning = false;



		////////////////

		public bool TryGetPlayerPPSpendingSync( string key, out float value ) {
			lock( this.MyLock ) {
				return this.PlayerPPSpendings.TryGetValue( key, out value );
			}
		}

		public void SetPlayerPPSpendingSync( string key, float value ) {
			lock( this.MyLock ) {
				this.PlayerPPSpendings[key] = value;
			}
		}

		public void AddPlayerPPSpendingSync( string key, float value ) {
			lock( this.MyLock ) {
				if( this.PlayerPPSpendings.ContainsKey( key ) ) {
					this.PlayerPPSpendings[key] += value;
				} else {
					this.PlayerPPSpendings[key] = value;
				}
			}
		}

		////////////////

		internal void ResetAll() {
			this.PlayersValidated.Clear();
			this.AllPlayedWorlds.Clear();
			this.PlayerPPSpendings.Clear();
			this.CurrentSessionedWorldId = "";
			this.AwaitingNextWorld = false;
			this.IsRunning = false;

			if( ResetModeMod.Instance.Config.DebugModeInfo ) {
				LogHelpers.Alert();
			}
		}


		internal ResetModeSessionData Clone() {
			return (ResetModeSessionData)this.MemberwiseClone();
		}

		////////////////

		public override string ToString() {
			return JsonConvert.SerializeObject( this, Formatting.Indented );
		}
	}
}
