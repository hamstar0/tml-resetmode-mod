using System.Collections.Generic;


namespace ResetMode.Data {
	public class ResetModeSessionData {
		public ISet<string> PlayersValidated = new HashSet<string>();
		public IDictionary<string, float> PlayerPPSpendings = new Dictionary<string, float>();

		public ISet<string> AllPlayedWorlds = new HashSet<string>();
		public string CurrentWorld = "";

		public bool AwaitingNextWorld = false;
		public bool IsRunning = false;



		////////////////
		
		internal void ResetAll() {
			this.PlayersValidated.Clear();
			this.AllPlayedWorlds.Clear();
			this.PlayerPPSpendings.Clear();
			this.CurrentWorld = "";
			this.AwaitingNextWorld = false;
			this.IsRunning = false;
		}
	}
}
