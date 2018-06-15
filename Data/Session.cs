using System.Collections.Generic;


namespace ResetMode.Data {
	public class ResetModeSessionData {
		//public IDictionary<string, int> PlayerHashes = new Dictionary<string, int>();
		public IDictionary<string, float> PlayerPPSpendings = new Dictionary<string, float>();

		public ISet<string> AllPlayedWorlds = new HashSet<string>();
		public bool AwaitingNextWorld = false;
		public bool IsRunning = false;



		////////////////
		
		internal void ResetAll() {
			this.AllPlayedWorlds.Clear();
			this.PlayerPPSpendings.Clear();
			this.AwaitingNextWorld = false;
			this.IsRunning = false;
		}
	}
}
